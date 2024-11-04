namespace CVRecruitment.ViewModels
{
    public class CompanyViewModel
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string CompanyType { get; set; }
        public string CompanySize { get; set; }
        public string CompanyCountry { get; set; }
        public string WorkingDay { get; set; }
        public int OvertimePolicy { get; set; }
        public string Email { get; set; }
        public string HotLine { get; set; }
        public string Website { get; set; }
        public IFormFile Logo { get; set; }
        public IFormFile[] CompanyImages { get; set; }
    }
}
