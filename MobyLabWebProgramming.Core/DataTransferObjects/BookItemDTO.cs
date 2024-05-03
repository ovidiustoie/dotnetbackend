namespace MobyLabWebProgramming.Core.DataTransferObjects;
public class BookItemDTO
{
    public Guid Id { get; set; }
    public string BarCode { get; set; } = default!;
    public string BookTitle { get; set; } = default!;
    public Guid BookId { get; set; } = default!;

}