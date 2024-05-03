namespace MobyLabWebProgramming.Core.DataTransferObjects;
public class BookDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Authors { get; set; } = default!;

}