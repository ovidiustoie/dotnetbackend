namespace MobyLabWebProgramming.Core.DataTransferObjects;
public record BookUpdateDTO(Guid Id, string? Title = default, string? Summary = default, ICollection<AuthorRefDTO>? Authors = default);
