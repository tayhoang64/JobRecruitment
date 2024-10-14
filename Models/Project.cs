using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public int? IsDoing { get; set; }

    public int? StartMonth { get; set; }

    public int? StartYear { get; set; }

    public int? EndMonth { get; set; }

    public int? EndYear { get; set; }

    public string? ShortDescription { get; set; }

    public string? ProjectUrl { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
