using System.Collections.Generic;
using System.Net;
using System.Text;
using Ardalis.Specification;
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
        var result = await _repository.GetAsync(new BookProjectionSpec(id), cancellationToken); //

        return result != null ?
            ServiceResponse<BookAddDTO>.ForSuccess(result) :
            ServiceResponse<BookAddDTO>.FromError(CommonErrors.AuthorNotFound);
    }

    public async Task<ServiceResponse<PagedResponse<BookAddDTO>>> GetBooks(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new BookProjectionSpec(pagination.Search), cancellationToken);
        return ServiceResponse<PagedResponse<BookAddDTO>>.ForSuccess(result);
    }

    
    public async Task<ServiceResponse<int>> GetBookCount(CancellationToken cancellationToken = default) =>
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<User>(cancellationToken));

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
        };
        await _repository.DbContext.Set<Book>().AddAsync(newBook, cancellationToken);
        foreach (var author in book.Authors)
        {
            await _repository.DbContext.Set<BookAuthor>().AddAsync(new BookAuthor
            {
                BookId = newBook.Id,
                AuthorId = author.Id,
            }, cancellationToken);
        }
        await _repository.DbContext.SaveChangesAsync(cancellationToken); 
     
        return ServiceResponse.ForSuccess();
    }

    

    public async Task<ServiceResponse> DeleteBook(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) 
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can delete books!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<Author>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
   
}


