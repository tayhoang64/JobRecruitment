using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CVRecruitment.Models;

public partial class CompanyImage
{
    public int CompanyImageId { get; set; }

    public string? File { get; set; }

    public int CompanyId { get; set; }
    [JsonIgnore]
    public virtual Company Company { get; set; } = null!;
}
