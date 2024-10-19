using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CVRecruitment.Models;

public partial class User : IdentityUser<int>
{
    public bool EmailConfirmed { get; set; } = false;
    public string? FullName { get; set; }

    public string? Title { get; set; }

    public string? EmailAddress { get; set; }

    public int? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? PersonalLink { get; set; }

    public string? Avatar { get; set; }

    public string? AboutMe { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual ICollection<MySkill> MySkills { get; set; } = new List<MySkill>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();

    public virtual ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
}
