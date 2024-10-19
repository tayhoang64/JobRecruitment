using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
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
    public class CompanyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly Cloudinary _cloudinary;

        public CompanyController(IConfiguration configuration, CvrecruitmentContext context, Cloudinary cloudinary)
        {
            _configuration = configuration;
            _context = context;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == id && c.ConfirmCompany == true);
            if(company == null)
            {
                return NotFound("Not found or haven't been confirm yet");
            }

            return Ok(company);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Company>> SignUpCompany(
                                         Company company,
                                         IFormFile logo,
                                         IFormFile[] companyImages) 
        {
            Company NewCompany = new Company()
            {
                CompanyName = company.CompanyName,
                Address = company.Address,
                Description = company.Description,
                CompanyType = company.CompanyType,
                CompanySize = company.CompanySize,
                CompanyCountry = company.CompanyCountry,
                WorkingDay = company.WorkingDay,
                OvertimePolicy = company.OvertimePolicy,
                ConfirmCompany = false
            };
            if(logo != null)
            {
                NewCompany.Logo = await UploadToStorage(logo);
            }

            if (companyImages != null && companyImages.Length > 0)
            {
                foreach (var image in companyImages)
                {
                    var imageUrl = await UploadToStorage(image);
                    var companyImage = new CompanyImage { File = imageUrl };
                    NewCompany.CompanyImages.Add(companyImage);
                }
            }
            _context.Companies.Add(NewCompany);
            await _context.SaveChangesAsync();
            return Ok(company);
        }

        private async Task<string> UploadToStorage(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Crop("fill").Gravity("center")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception("Upload failed: " + uploadResult.Error.Message);
                }
            }
        }
    }
}
