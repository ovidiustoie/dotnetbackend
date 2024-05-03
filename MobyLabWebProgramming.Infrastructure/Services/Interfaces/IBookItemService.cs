using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;
public interface IBookItemService
{
    public Task<ServiceResponse<BookItemDTO>> GetBookItem(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<PagedResponse<BookItemDTO>>> GetBookItems(Guid bookId, PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddBookItem(BookItemDTO bookItem, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateBookItem(BookItemUpdateDTO bookItem, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteBookItem(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
}
