using CVRecruitment.Models;

namespace CVRecruitment.ViewModels
{
    public class TemplateViewModel
    {
        public int? TemplateId { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }  
        public int? UploadedBy { get; set; }
    }

    public class TemplateResponse
    {
        public int TemplateId { get; set; }

        public string? Title { get; set; }

        public string? File { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? UploadedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public string? Image { get; set; }

        public User? User { get; set; }
    }
}
