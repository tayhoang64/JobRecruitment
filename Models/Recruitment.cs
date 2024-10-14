using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Recruitment
{
    public int UserId { get; set; }

    public int JobId { get; set; }

    public string? FileCv { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual Job Job { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
