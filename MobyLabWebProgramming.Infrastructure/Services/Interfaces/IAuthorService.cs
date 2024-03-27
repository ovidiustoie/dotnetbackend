using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be uses to mange author information.
/// As most routes and business logic will need to know what author is currently using the backend this service will be the most used.
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// GetAuthor will provide the information about a aurhor given its aurhor Id.
    /// </summary>
    public Task<ServiceResponse<AuthorDTO>> GetAuthor(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetAuthors returns page with aurhor information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<AuthorDTO>>> GetAuthors(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetAuthorCount returns the number of authors in the database.
    /// </summary>
    public Task<ServiceResponse<int>> GetAuthorCount(CancellationToken cancellationToken = default);
    /// <summary>
    /// AddAuthor adds an author and verifies if requesting author has permissions to add one.
    /// If the requesting author is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> AddAuthor(AuthorAddDTO author, UserDTO? requestingAuthor = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdateAuthor updates an author and verifies if requesting author has permissions to update it, if the author is his own then that should be allowed.
    /// If the requesting author is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> UpdateAuthor(AuthorUpdateDTO author, UserDTO? requestingAuthor = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeleteAuthor deletes an author and verifies if requesting author has permissions to delete it, if the author is his own then that should be allowed.
    /// If the requesting author is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> DeleteAuthor(Guid id, UserDTO? requestingAuthor = default, CancellationToken cancellationToken = default);
}
