using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.Entities;

public class Feedback : BaseEntity
{
    public int Score { get; set; } = default!;
    public string Comment { get; set; } = default!;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
