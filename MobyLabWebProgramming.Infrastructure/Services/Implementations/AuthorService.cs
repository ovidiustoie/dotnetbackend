using System.Net;
using MobyLabWebProgramming.Core.Constants;
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

public class AuthorService : IAuthorService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;
  
    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public AuthorService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<AuthorDTO>> GetAuthor(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new AuthorProjectionSpec(id), cancellationToken); // Get a user using a specification on the repository.

        return result != null ?
            ServiceResponse<AuthorDTO>.ForSuccess(result) :
            ServiceResponse<AuthorDTO>.FromError(CommonErrors.AuthorNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<AuthorDTO>>> GetAuthors(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new AuthorProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse<PagedResponse<AuthorDTO>>.ForSuccess(result);
    }

    
    public async Task<ServiceResponse<int>> GetAuthorCount(CancellationToken cancellationToken = default) =>
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<User>(cancellationToken)); // Get the count of all user entities in the database.

    public async Task<ServiceResponse> AddAuthor(AuthorAddDTO author, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Clients can't add authors", ErrorCodes.CannotAdd));
        }
        var fullName = author.FirstName + ' ' + author.LastName.ToUpper();  

        var result = await _repository.GetAsync(new AuthorSpec(fullName), cancellationToken);

        if (result != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The author already exists!", ErrorCodes.UserAlreadyExists));
        }

        await _repository.AddAsync(new Author
        {
            FirstName = author.FirstName,
            LastName = author.LastName,
            Description = author.Description,
            FullName = fullName
        }, cancellationToken); // A new entity is created and persisted in the database.

     
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateAuthor(AuthorUpdateDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin  or personal can update the authors!", ErrorCodes.CannotUpdate));
        }

        var entity = await _repository.GetAsync(new AuthorSpec(user.Id), cancellationToken);

        if (entity != null) // Verify if the user is not found, you cannot update an non-existing entity.
        {
            entity.FirstName = user.FirstName ?? entity.FirstName;
            entity.LastName = user.LastName ?? entity.LastName;
            entity.Description = user.Description ?? entity.Description;
            entity.FullName = entity.FirstName + ' ' + entity.LastName.ToUpper();
            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAuthor(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin  or personal can delete the authors!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<User>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}


