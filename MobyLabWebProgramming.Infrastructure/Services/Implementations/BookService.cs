﻿using System.Collections.Generic;
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
using System.Linq;
using System.Collections;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class BookService : IBookService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    public BookService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }
    public async Task<ServiceResponse<int>> GetBookCount(CancellationToken cancellationToken = default) =>
       ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<Book>(cancellationToken));

    public async Task<ServiceResponse<BookAddDTO>> GetBook(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync(new BookSpec(id), cancellationToken);
        if (entity != null)
        {
            var res = new BookAddDTO()
            {
                Title = entity.Title,
                Summary = entity.Summary,
                Id = entity.Id,
            };
            res.Authors = new List<AuthorRefDTO>();
            if (entity.Authors != null)
            {
                foreach (var author in entity.Authors)
                {
                    res.Authors.Add(new AuthorRefDTO() { FullName = author.FullName, Id = author.Id });
                }
            }
            return ServiceResponse<BookAddDTO>.ForSuccess(res);
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
        if (book.Authors == null || book.Authors.Count == 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Authors are mandatory!", ErrorCodes.CannotUpdate));
        }

        var entity = await _repository.GetAsync(new BookSpec(book.Id), cancellationToken);


        if (entity != null)
        {
            entity.Title = book.Title ?? entity.Title;
            entity.Summary = book.Summary ?? entity.Summary;
            var map = new Dictionary<Guid, Author>();
            foreach (var author in entity.Authors)
            {
                map.Add(author.Id, author);
            }

            foreach (var author in book.Authors)
            {
                if (map.ContainsKey(author.Id))
                {
                    map.Remove(author.Id);
                }
                else
                {
                    var authorDb = await _repository.GetAsync<Author>(new AuthorSpec(author.Id), cancellationToken);
                    if (authorDb != null)
                    {
                        entity.Authors.Add(authorDb);
                    }
                }
            }

            foreach (var author in map.Values)
            {
                entity.Authors.Remove(author);
            }
            await _repository.UpdateAsync(entity);
        }

        return ServiceResponse.ForSuccess();
    }

}


