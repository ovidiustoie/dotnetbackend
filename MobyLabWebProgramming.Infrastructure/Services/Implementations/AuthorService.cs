using System.Net;
using System.Text;
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
        string fullName = AuthorService.getFullName(author.FirstName, author.LastName);
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
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) 
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can update the authors!", ErrorCodes.CannotUpdate));
        }

        var entity = await _repository.GetAsync(new AuthorSpec(user.Id), cancellationToken);

        if (entity != null) 
        {
            entity.FirstName = user.FirstName ?? entity.FirstName;
            entity.LastName = user.LastName ?? entity.LastName;
            entity.Description = user.Description ?? entity.Description;
            entity.FullName = AuthorService.getFullName(entity.FirstName, entity.LastName);
            await _repository.UpdateAsync(entity, cancellationToken); 
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteAuthor(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role == UserRoleEnum.Client) 
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can delete the authors!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<User>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }

    public static string getFullName(string firstName, string lastName)
    {
        StringBuilder fullName = new StringBuilder();
        bool addSpace = true;
        if (!string.IsNullOrEmpty(firstName))
        {
            fullName.Append(firstName);

        } else
        {
            addSpace = false;
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            if (addSpace)
            {
                fullName.Append(" ");
            }
            fullName.Append(lastName.ToUpper());

        }
        return fullName.ToString();
    }
}


