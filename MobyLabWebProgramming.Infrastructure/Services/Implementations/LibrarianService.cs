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

public class LibrarianService : ILibrarianService

{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public LibrarianService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<LibrarianDTO>> GetLibrarian(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new LibrarianProjectionSpec(id), cancellationToken); 

        return result != null ?
            ServiceResponse<LibrarianDTO>.ForSuccess(result) :
            ServiceResponse<LibrarianDTO>.FromError(CommonErrors.LibrarianNotFound); 
    }

    public async Task<ServiceResponse<PagedResponse<LibrarianDTO>>> GetLibrarians(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new LibrarianProjectionSpec(pagination.Search), cancellationToken); 

        return ServiceResponse<PagedResponse<LibrarianDTO>>.ForSuccess(result);
    }


    public async Task<ServiceResponse<int>> GetLibrarianCount(CancellationToken cancellationToken = default) =>
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<Librarian>(cancellationToken)); 

    public async Task<ServiceResponse> AddLibrarian(LibrarianAddDTO librarian, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin) 
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can create the librarians", ErrorCodes.CannotAdd));
        }
        if (String.IsNullOrEmpty(librarian.FirstName))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "First name is mandatory", ErrorCodes.CannotAdd));

        }
        if (String.IsNullOrEmpty(librarian.LastName))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Last name is mandatory", ErrorCodes.CannotAdd));

        }
        if (String.IsNullOrEmpty(librarian.Email))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Email is mandatory", ErrorCodes.CannotAdd));

        }
        var euser = await _repository.GetAsync(new UserSpec(librarian.Email), cancellationToken);

        if (euser != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The user already exists!", ErrorCodes.UserAlreadyExists));
        }
        var user = new User()
        {
            Email = librarian.Email,
            Name = librarian.FirstName,
            Role = UserRoleEnum.Personnel,
            Password = PasswordUtils.HashPassword(librarian.Email)
        };
        var librarianDB = new Librarian()
        {
            User = user,
            FirstName = librarian.FirstName,
            LastName = librarian.LastName,
            Position = librarian.Position,
            Description = librarian.Description,
        };
        await _repository.AddAsync(librarianDB,  cancellationToken); // A new entity is created and persisted in the database.
  
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateLibrarian(LibrarianUpdateDTO librarian, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or personal can update the librarian!", ErrorCodes.CannotUpdate));
        }
        var entity = await _repository.GetAsync(new LibrarianSpec(librarian.Id, null), cancellationToken);

        if (entity != null)
        {
            entity.FirstName = librarian.FirstName ?? entity.FirstName;
            entity.LastName = librarian.LastName ?? entity.LastName;
            entity.Position = librarian.Position ?? entity.Position;
            entity.Description = librarian.Description ?? entity.Description;
            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }
   
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteLibrarian(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can delete the librarians!", ErrorCodes.CannotDelete));
        }
        var librarianDb = await _repository.GetAsync(new LibrarianSpec(id, null), cancellationToken);
        if (librarianDb != null) {
            if (librarianDb.User != null)
            {
                _repository.DbContext.Remove(librarianDb.User);
            }
            _repository.DbContext.Remove(librarianDb); // And remove it.

            await _repository.DbContext.SaveChangesAsync(cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }
}


