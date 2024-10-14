using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? CompanyType { get; set; }

    public string? CompanySize { get; set; }

    public string? CompanyCountry { get; set; }

    public string? WorkingDay { get; set; }

    public int? OvertimePolicy { get; set; }

    public string? Logo { get; set; }

    public virtual ICollection<CompanyImage> CompanyImages { get; set; } = new List<CompanyImage>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
