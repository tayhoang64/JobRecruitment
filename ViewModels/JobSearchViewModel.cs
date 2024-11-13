namespace CVRecruitment.ViewModels
{
    public class JobSearchViewModel
    {
        public string? JobName { get; set; } = string.Empty;
        public string? Location { get; set; } = null;
        public string? Salary { get; set; } = null;
        public int WorkExp { get; set; } = 0;
        public List<int> SkillIds { get; set; } = new List<int>();
    }
}
