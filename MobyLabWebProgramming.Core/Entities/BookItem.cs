
namespace MobyLabWebProgramming.Core.Entities;

public class BookItem : BaseEntity
{
    public string BarCode { get; set; } = default!;
    public Guid BookId { get; set; }
    public Book Book { get; set; } = default!;
}
