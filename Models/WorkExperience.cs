using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class WorkExperience
{
    public int Weid { get; set; }

    public string? JobTitle { get; set; }

    public string? Company { get; set; }

    public int? IsWorking { get; set; }

    public int? FromMonth { get; set; }

    public int? FromYear { get; set; }

    public int? ToMonth { get; set; }

    public int? ToYear { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
