using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public CertificatesController(IConfiguration configuration, CvrecruitmentContext context, FileService fileService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _fileService = fileService;
            _userManager = userManager;
        }

        private async Task<(Models.User user, IActionResult result)> CheckLogin()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            return (user, null);
        }


        // Create a certificate
        [HttpPost]
        public async Task<IActionResult> CreateCertificate(ViewModels.CertificatesViewModel certificate)
        {
            var (user, result) = await CheckLogin();
            if(result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Certificate newCertificate = new Certificate()
            {
                CertificateName = certificate.CertificateName,
                Organization = certificate.Organization,
                IssueMonth = certificate.IssueMonth,
                IssueYear = certificate.IssueYear,
                CertificateUrl = certificate.CertificateUrl,
                Description = certificate.Description,
                UserId = user.Id,
            };

            _context.Certificates.Add(newCertificate);
            await _context.SaveChangesAsync();

            return Ok(newCertificate);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCertificates()
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            return Ok(_context.Certificates.Where(c=>c.UserId==user.Id));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificates(int id)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var certificate = _context.Certificates.FirstOrDefault(certificate => certificate.CertificateId ==id);
            if (certificate == null) {
                return NotFound("Certificates not found");
            }
            if (certificate.UserId != user.Id) {
                return Forbid();
            }
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return Ok("Finish!!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertificates(int id, CertificatesViewModel UpdateCertificates)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var certificate = _context.Certificates.FirstOrDefault(certificate => certificate.CertificateId == id);
            if (certificate == null)
            {
                return NotFound("Certificates not found");
            }
            if (certificate.UserId != user.Id)
            {
                return Forbid();
            }
            certificate.CertificateName = UpdateCertificates.CertificateName;
            certificate.Organization = UpdateCertificates.Organization;
            certificate.IssueMonth = UpdateCertificates.IssueMonth;
            certificate.IssueYear = UpdateCertificates.IssueYear;
            certificate.CertificateUrl = UpdateCertificates.CertificateUrl;
            certificate.Description = UpdateCertificates.Description;
            await _context.SaveChangesAsync();
            return Ok("Updated succesfully");
        }
    }
}
