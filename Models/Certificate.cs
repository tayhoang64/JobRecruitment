using System;
using System.Collections.Generic;

namespace CVRecruitment.Models;

public partial class Certificate
{
    public int CertificateId { get; set; }

    public string? CertificateName { get; set; }

    public string? Organization { get; set; }

    public int? IssueMonth { get; set; }

    public int? IssueYear { get; set; }

    public string? CertificateUrl { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
