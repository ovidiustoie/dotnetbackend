
namespace MobyLabWebProgramming.Core.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public ICollection<Author> Authors { get; set; } = default!;
    public ICollection<BookItem> BookItems { get; set; } = default!;
}

