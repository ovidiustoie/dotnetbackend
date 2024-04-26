using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the user entities and map it to and UserDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class BookProjectionSpec : BaseSpec<BookProjectionSpec, Book, BookAddDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get UserDTO object from the database.
    /// </summary>
    protected override Expression<Func<Book, BookAddDTO>> Spec => e => new()
    {
        Id = e.Id,
        Title = e.Title,
        Summary = e.Summary,

    };

    public BookProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public BookProjectionSpec(Guid id) {
        Query.Select(Derived.Spec).Include(b => b.Authors).Where(e => e.Id == id);
    }

    public BookProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Title, searchExpr));
    }
}
