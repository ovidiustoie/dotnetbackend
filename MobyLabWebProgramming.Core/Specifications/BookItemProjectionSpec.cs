using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class BookItemProjectionSpec : BaseSpec<BookItemProjectionSpec, BookItem, BookItemDTO>
{
    protected override Expression<Func<BookItem, BookItemDTO>> Spec => e => new()
    {
        Id = e.Id,
        BarCode = e.BarCode,
        BookId = e.BookId,
        BookTitle = e.Book.Title,
    };

    public BookItemProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public BookItemProjectionSpec(Guid id)
    {
        Query.Where(e => e.Id == id).Include(x => x.Book);
    }


    public BookItemProjectionSpec(Guid bookId, string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.BarCode, searchExpr)).Include(x => x.Book);                                                            // Note that this will be translated to the database something like "where user.Name ilike '%str%'".
    }
}
