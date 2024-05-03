using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;
public interface IBookService
{
    public Task<ServiceResponse<BookAddDTO>> GetBook(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<PagedResponse<BookDTO>>> GetBooks(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddBook(BookAddDTO book, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteBook(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateBook(BookUpdateDTO book, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<int>> GetBookCount(CancellationToken cancellationToken = default);

}
