using CVRecruitment.Models;

namespace CVRecruitment.ViewModels
{
    public class JobViewModel
    {
        public string? JobName { get; set; }

        public string? Salary { get; set; }

        public string? Location { get; set; }

        public string? WorkStyle { get; set; }

        public string? Description { get; set; }

        public DateTime? EndDay { get; set; }
        public int? ExperienceYear { get; set; }
        public int? RecruitmentCount { get; set; }

        public int? CompanyId { get; set; }

        public List<int> SkillIds { get; set; }
    }
}
