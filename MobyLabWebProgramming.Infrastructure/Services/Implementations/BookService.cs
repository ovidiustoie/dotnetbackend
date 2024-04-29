using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Data.SqlClient;
using MobyLabWebProgramming.Infrastructure.Repositories.Implementation;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class BookService : IBookService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    public BookService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<BookAddDTO>> GetBook(Guid id, CancellationToken cancellationToken = default)
    {

        using var cmd = _repository.DbContext.Database.GetDbConnection().CreateCommand();
        var sql =
 $@"select json_agg(root) as result from(SELECT *,
    (select json_agg(Authors) from(
         select ba.""AuthorId"" as ""Id"", a.""FullName"" as ""FullName"" from

              ""BookAuthor"" as ba inner join ""Author"" as a on ba.""AuthorId"" = a.""Id""

         where ba.""BookId"" = book.""Id""
       ) as Authors
    ) as ""Authors""
FROM ""Book"" book where  book.""Id"" = @Id) root";


        var jsonData = "";
        if (cmd.Connection != null)
        {
            await cmd.Connection.OpenAsync(cancellationToken);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "Id";
            parameter.Value = id;
            cmd.Parameters.Add(parameter);
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                jsonData = reader["result"].ToString();

        }
        if (!String.IsNullOrEmpty(jsonData))
        {
            var items = JsonSerializer.Deserialize<BookAddDTO[]>(jsonData);
            if (items != null && items.Length > 0)
            {
                return ServiceResponse<BookAddDTO>.ForSuccess(items[0]);
            }
        }
        return ServiceResponse<BookAddDTO>.FromError(CommonErrors.BookNotFound);
    }

    public async Task<ServiceResponse<PagedResponse<BookDTO>>> GetBooks(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        StringBuilder sql = new StringBuilder();
        StringBuilder sqlCount = new StringBuilder();
        sql.Append(
 $@"with ""authorsOfBooks"" as (
	select ba.""BookId"", STRING_AGG(a.""FullName"", ',') as ""Authors"" from ""BookAuthor"" as ba 
	inner join ""Author"" as a on ba.""AuthorId"" = a.""Id""
	group by ba.""BookId""
)
select json_agg(root) as result from (SELECT book.*, ab.""Authors""
FROM ""Book"" book 
left outer join ""authorsOfBooks"" ab on ab.""BookId"" = book.""Id""
"
);

        sqlCount.Append(
$@"with ""authorsOfBooks"" as (
	select ba.""BookId"", STRING_AGG(a.""FullName"", ',') as ""Authors"" from ""BookAuthor"" as ba 
	inner join ""Author"" as a on ba.""AuthorId"" = a.""Id""
	group by ba.""BookId""
)
SELECT count(*) as ""Count""
FROM ""Book"" book 
left outer join ""authorsOfBooks"" ab on ab.""BookId"" = book.""Id""
"
);
        if (!String.IsNullOrEmpty(pagination.Search))
        {
            sql.Append(" where book.\"Title\" ILIKE @Search OR ab.\"Authors\" ILIKE  @Search");
            sqlCount.Append(" where book.\"Title\" ILIKE @Search OR ab.\"Authors\" ILIKE  @Search");
        }

        sql.Append(" LIMIT " + pagination.PageSize);
        if (pagination.Page > 1)
        {
            sql.Append(" OFFSET " + (pagination.Page - 1) * pagination.PageSize);
        }
        sql.Append(" ) root");

        var jsonData = "";
        uint count = 0;
        using var cmd = _repository.DbContext.Database.GetDbConnection().CreateCommand();
        if (cmd.Connection != null)
        {
            if (cmd.Connection.State != ConnectionState.Open)
                await cmd.Connection.OpenAsync(cancellationToken);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql.ToString();
            if (!String.IsNullOrEmpty(pagination.Search))
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = "Search";
                parameter.Value = "%" + pagination.Search + "%";
                cmd.Parameters.Add(parameter);
            }
            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
                jsonData = reader["result"].ToString();
        }
        using var cmdCount = _repository.DbContext.Database.GetDbConnection().CreateCommand();
        if (cmdCount.Connection != null)
        {
            if (cmdCount.Connection.State != ConnectionState.Open)
                await cmdCount.Connection.OpenAsync(cancellationToken);
            cmdCount.CommandType = CommandType.Text;
            cmdCount.CommandText = sqlCount.ToString();
            if (!String.IsNullOrEmpty(pagination.Search))
            {
                var parameter = cmdCount.CreateParameter();
                parameter.ParameterName = "Search";
                parameter.Value = "%" + pagination.Search + "%";
                cmdCount.Parameters.Add(parameter);
            }

            using var readerCount = await cmdCount.ExecuteReaderAsync(cancellationToken);
            if (await readerCount.ReadAsync(cancellationToken))
                count = (uint)readerCount.GetInt32(0);

        }
        var res = new PagedResponse<BookDTO>(pagination.Page, pagination.PageSize, count, new List<BookDTO> { });
        if (!String.IsNullOrEmpty(jsonData))
        {
            var arrayDTO = JsonSerializer.Deserialize<BookDTO[]>(jsonData);
            if (arrayDTO != null && arrayDTO.Length > 0)
            {

                res = new PagedResponse<BookDTO>(pagination.Page, pagination.PageSize, count, arrayDTO.ToList());
            }

        }
        return ServiceResponse<PagedResponse<BookDTO>>.ForSuccess(res);
    }

    public async Task<ServiceResponse> AddBook(BookAddDTO book, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Clients can't add books", ErrorCodes.CannotAdd));
        }
        var newBook = new Book
        {
            Title = book.Title,
            Summary = book.Summary,
            Authors = new Collection<Author>(),
        };
        if (book.Authors == null || book.Authors.Count == 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Authors are mandatory", ErrorCodes.CannotAdd));
        }
        foreach (var author in book.Authors)
        {
            var authorDB = await _repository.GetAsync<Author>(author.Id, cancellationToken);
            if (authorDB != null)
                newBook.Authors.Add(authorDB);
        }
        await _repository.AddAsync(newBook);

        return ServiceResponse.ForSuccess();
    }



    public async Task<ServiceResponse> DeleteBook(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can delete books!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<Book>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateBook(BookUpdateDTO book, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can update the books!", ErrorCodes.CannotUpdate));
        }

        var entity = await _repository.GetAsync(new BookSpec(book.Id), cancellationToken);

        if (entity != null)
        {
            entity.Title = book.Title ?? entity.Title;
            entity.Summary = book.Summary ?? entity.Summary;
            if (book.Authors != null && book.Authors.Count == 0)
            {
                foreach (var author in book.Authors)
                {
                    var authorDB = await _repository.GetAsync<Author>(author.Id, cancellationToken);
                    if (authorDB != null)
                        entity.Authors.Add(authorDB);
                }
            }
            
            await _repository.UpdateAsync(entity, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

}


