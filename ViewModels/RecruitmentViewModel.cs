using CVRecruitment.Models;

namespace CVRecruitment.ViewModels
{
    public class RecruitmentViewModel
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public IFormFile? FormFile { get; set; }

    }

    public class RecruitmentResponse
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public string? FileCV { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
