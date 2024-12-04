namespace CVRecruitment.ViewModels
{
    public class CertificatesViewModel
    {
        public string? CertificateName { get; set; }

        public string? Organization { get; set; }

        public int? IssueMonth { get; set; }

        public int? IssueYear { get; set; }

        public string? CertificateUrl { get; set; }

        public string? Description { get; set; }
    }
    public class CertificatesResponse
    {
        public int CertificateId { get; set; }
        public string? CertificateName { get; set; }

        public string? Organization { get; set; }

        public int? IssueMonth { get; set; }

        public int? IssueYear { get; set; }

        public string? CertificateUrl { get; set; }

        public string? Description { get; set; }
        public int? UserId { get; set; }
    }
}
