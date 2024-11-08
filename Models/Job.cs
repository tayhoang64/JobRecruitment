using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Job
{
    public int JobId { get; set; }

    public string? JobName { get; set; }

    public string? Salary { get; set; }

    public string? Location { get; set; }

    public string? WorkStyle { get; set; }

    public DateTime? PostedDay { get; set; }

    public string? Description { get; set; }

    public DateTime? EndDay { get; set; }
    public int? ExperienceYear { get; set; }
    public int? RecruitmentCount { get; set; }
    public bool Status { get; set; } = true;
    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public virtual Company Company { get; set; } = null!;
    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
