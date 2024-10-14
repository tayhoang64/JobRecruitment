using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string? SkillName { get; set; }

    public virtual ICollection<MySkill> MySkills { get; set; } = new List<MySkill>();

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
