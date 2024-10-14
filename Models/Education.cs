using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Education
{
    public int EducationId { get; set; }

    public string? School { get; set; }

    public string? Major { get; set; }

    public int? IsStudying { get; set; }

    public DateTime? FromMonth { get; set; }

    public DateTime? FromYear { get; set; }

    public DateTime? ToMonth { get; set; }

    public DateTime? ToYear { get; set; }

    public string? AdditionalDetail { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
