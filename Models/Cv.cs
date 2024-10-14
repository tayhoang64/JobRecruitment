using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Cv
{
    public int Cvid { get; set; }

    public int UserId { get; set; }

    public string? File { get; set; }

    public DateTime? LastUpdateAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int TemplateId { get; set; }

    public virtual Template Template { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
