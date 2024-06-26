﻿using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.Entities;

public class Librarian : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Position { get; set; } = default!;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
