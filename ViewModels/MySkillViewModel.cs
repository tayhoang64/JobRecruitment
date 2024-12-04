
namespace CVRecruitment.ViewModels
{
    public class MySkillViewModel
    {
        public List<MySkillSub> SkillIds { get; set; } = new List<MySkillSub>();
    }
    public class MySkillResponse
    {
        public int SkillId { get; set; }
        public string? SkillName { get; set; }
        public string? Level {  get; set; }
    }
    public class MySkillSub
    {
        public int SkillId { get; set; }
        public String? Level { get; set; }
    }
}
