using CVRecruitment.Models;
using CVRecruitment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public CertificatesController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCertificate([FromBody] Certificate certificate)
        {
            if (certificate == null)
            {
                return BadRequest("Certificate data is required.");
            }

            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Certificate created successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCertificates()
        {
            var certificates = await _context.Certificates.ToListAsync();
            return Ok(certificates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCertificateById(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound("Certificate not found.");
            }
            return Ok(certificate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertificate(int id, [FromBody] Certificate updatedCertificate)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound("Certificate not found.");
            }

            // Cập nhật thông tin
            certificate.CertificateName = updatedCertificate.CertificateName;
            certificate.Organization = updatedCertificate.Organization;
            certificate.IssueMonth = updatedCertificate.IssueMonth;
            certificate.IssueYear = updatedCertificate.IssueYear;
            certificate.CertificateUrl = updatedCertificate.CertificateUrl;
            certificate.Description = updatedCertificate.Description;
            certificate.UserId = updatedCertificate.UserId;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Certificate updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound("Certificate not found.");
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Certificate deleted successfully!" });
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadCertificate([FromForm] IFormFile file, int userId)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("File is required.");
        //    }

        //    var uploadResult = await _cloudinaryService.UploadFileAsync(file);
        //    if (uploadResult == null)
        //    {
        //        return StatusCode(500, "Error uploading file.");
        //    }

        //    var certificate = new Certificate
        //    {
        //        CertificateName = file.FileName,
        //        CertificateUrl = uploadResult.Url,
        //        UserId = userId
        //    };

        //    await _context.Certificates.AddAsync(certificate);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Certificate uploaded successfully!", Certificate = certificate });
        //}

    }
}
