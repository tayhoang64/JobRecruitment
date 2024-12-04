namespace CVRecruitment.ViewModels
{
    public class ProjectViewModel
    {

        public string? ProjectName { get; set; }

        public int? IsDoing { get; set; }

        public int? StartMonth { get; set; }

        public int? StartYear { get; set; }

        public int? EndMonth { get; set; }

        public int? EndYear { get; set; }

        public string? ShortDescription { get; set; }

        public string? ProjectUrl { get; set; }
    }
    public class ProjectResponse
    {
        public int ProjectId { get; set; }

        public string? ProjectName { get; set; }

        public int? IsDoing { get; set; }

        public int? StartMonth { get; set; }

        public int? StartYear { get; set; }

        public int? EndMonth { get; set; }

        public int? EndYear { get; set; }

        public string? ShortDescription { get; set; }

        public string? ProjectUrl { get; set; }
        public int? UserId { get; set; }
    }
}
