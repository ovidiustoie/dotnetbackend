using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;
public interface IBookService
{
    public Task<ServiceResponse<BookAddDTO>> GetBook(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<PagedResponse<BookDTO>>> GetBooks(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddBook(BookAddDTO book, UserDTO? requestingAuthor = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteBook(Guid id, UserDTO? requestingAuthor = default, CancellationToken cancellationToken = default);
}
