using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class MySkill
{
    public int UserId { get; set; }

    public string? Level { get; set; }

    public int SkillId { get; set; }

    public virtual Skill Skill { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
