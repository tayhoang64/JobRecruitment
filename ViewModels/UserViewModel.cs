using CVRecruitment.Models;

namespace CVRecruitment.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public string? EmailAddress { get; set; }

        public string? Phone { get; set; }
        public List<string> Roles { get; set; }
    }
}
