using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class BookItemSpec : BaseSpec<BookItemSpec, BookItem>
{
    public BookItemSpec(Guid id) 
    {
        Query.Where(e => e.Id == id);
    }
 
}