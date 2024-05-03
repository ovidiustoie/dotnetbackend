namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class LibrarianDTO
{
    public Guid Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Position { get; set; } = default!;
}

