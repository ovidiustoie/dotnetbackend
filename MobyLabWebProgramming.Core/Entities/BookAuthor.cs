
using Microsoft.Extensions.Hosting;

namespace MobyLabWebProgramming.Core.Entities;

public class BookAuthor 
{
    public Guid AuthorId { get; set; }
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Author Author { get; set; } = null!;


}
