using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a simple specification to filter the user entities from the database via the constructors.
/// Note that this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class LibrarianSpec : BaseSpec<LibrarianSpec, Librarian>
{
    public LibrarianSpec(Guid? id, Guid? userId)
    {
        if (id != null)
        {
            Query.Where(e => e.Id == id).Include(x => x.User);
        }
        else if (userId != null)
        {
            Query.Where(e => e.UserId == userId).Include(x => x.User);
        }
    }
}
