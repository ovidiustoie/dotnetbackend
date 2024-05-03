using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class LibrarianProjectionSpec : BaseSpec<LibrarianProjectionSpec, Librarian, LibrarianDTO>
{
    protected override Expression<Func<Librarian, LibrarianDTO>> Spec => e => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Description = e.Description,
        Position = e.Position,
        Email = e.User.Email
    };

    public LibrarianProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public LibrarianProjectionSpec(Guid id)
    {
        Query.Where(e => e.Id == id).Include(x => x.User);
    }


    public LibrarianProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.FirstName, searchExpr)).Include(x => x.User);                                                            // Note that this will be translated to the database something like "where user.Name ilike '%str%'".
    }
}
