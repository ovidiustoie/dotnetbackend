
using Microsoft.Extensions.Hosting;

namespace MobyLabWebProgramming.Core.Entities;

public class BookAuthor 
{
    public Guid AuthorId { get; set; }
    public Guid BookId { get; set; }

}
