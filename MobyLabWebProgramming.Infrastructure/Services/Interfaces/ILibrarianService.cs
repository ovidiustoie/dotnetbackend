using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;
public interface ILibrarianService
{
    public Task<ServiceResponse<LibrarianDTO>> GetLibrarian(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<PagedResponse<LibrarianDTO>>> GetLibrarians(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<int>> GetLibrarianCount(CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddLibrarian(LibrarianAddDTO author, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateLibrarian(LibrarianUpdateDTO author, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteLibrarian(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
}
