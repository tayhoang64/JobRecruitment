using CVRecruitment.Models;

namespace CVRecruitment.ViewModels
{
    public class StaffViewModel
    {
        public string? Email { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public required List<int> RoleArray { get; set; }
    }

    public class CompanyRole
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class StaffResponse
    {
        public User User { get; set; }
        public Company Company { get; set; }
        public List<CompanyRole> Roles { get; set; }
    }
}
