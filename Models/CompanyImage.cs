using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class CompanyImage
{
    public int CompanyImageId { get; set; }

    public string? File { get; set; }

    public int CompanyId { get; set; }

    public virtual Company Company { get; set; } = null!;
}
