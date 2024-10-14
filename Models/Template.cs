using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Template
{
    public int TemplateId { get; set; }

    public string? Title { get; set; }

    public string? File { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
}
