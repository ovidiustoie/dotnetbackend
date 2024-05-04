using System.Data;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class BookItemService : IBookItemService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    public BookItemService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<BookItemDTO>> GetBookItem(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new BookItemProjectionSpec(id), cancellationToken);

        return result != null ?
            ServiceResponse<BookItemDTO>.ForSuccess(result) :
            ServiceResponse<BookItemDTO>.FromError(CommonErrors.BookItemNotFound);
    }

    public async Task<ServiceResponse<PagedResponse<BookItemDTO>>> GetBookItems(Guid bookId,  PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new BookItemProjectionSpec(bookId, pagination.Search), cancellationToken);

        return ServiceResponse<PagedResponse<BookItemDTO>>.ForSuccess(result);
    }


    public async Task<ServiceResponse> AddBookItem(BookItemDTO bookItem, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can create the book items", ErrorCodes.CannotAdd));
        }
        if (String.IsNullOrEmpty(bookItem.BarCode))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "BarCode name is mandatory", ErrorCodes.CannotAdd));

        }
        var book = await _repository.GetAsync(new BookSpec(bookItem.BookId), cancellationToken);

        if (book == null)
        {
            return ServiceResponse<BookItemDTO>.FromError(CommonErrors.BookNotFound);
        }
        var bookItemDB = new BookItem()
        {
            BarCode = bookItem.BarCode,
            Book = book,
        };
        await _repository.AddAsync(bookItemDB, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateBookItem(BookItemUpdateDTO bookItem, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can update book items!", ErrorCodes.CannotUpdate));
        }
        var entity = await _repository.GetAsync(new BookItemSpec(bookItem.Id), cancellationToken);

        if (entity != null)
        {
            entity.BarCode = bookItem.BarCode ?? entity.BarCode;
            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteBookItem(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can delete book items!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<BookItem>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}


