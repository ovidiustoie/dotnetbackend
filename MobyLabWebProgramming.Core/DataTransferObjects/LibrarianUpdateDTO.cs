namespace MobyLabWebProgramming.Core.DataTransferObjects;

public record LibrarianUpdateDTO(Guid Id, string? FirstName = default, string? LastName = default, string? Description = default, string? Position = default, string? Email = default);
