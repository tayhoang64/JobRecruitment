using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CVRecruitment.Models;
using CVRecruitment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CVRecruitment.ViewModels;
using System.Collections;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public CompanyController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Company>> SignUpCompany(CompanyViewModel companyViewModel)
        {
            //Check

            //Create
            Company NewCompany = new Company()
            {
                CompanyName = companyViewModel.CompanyName,
                Address = companyViewModel.Address,
                Description = companyViewModel.Description,
                CompanyType = companyViewModel.CompanyType,
                CompanySize = companyViewModel.CompanySize,
                CompanyCountry = companyViewModel.CompanyCountry,
                WorkingDay = companyViewModel.WorkingDay,
                OvertimePolicy = companyViewModel.OvertimePolicy,
                ConfirmCompany = false
            };
            if (companyViewModel.Logo != null)
            {
                try
                {
                    NewCompany.Logo = await _cloudinaryService.UploadImageAsync(companyViewModel.Logo, Enums.Avatars);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

            _context.Companies.Add(NewCompany);
            await _context.SaveChangesAsync();

            if (companyViewModel.CompanyImages != null && companyViewModel.CompanyImages.Length > 0)
            {
                foreach (var image in companyViewModel.CompanyImages)
                {
                    string imgUrl;
                    try
                    {
                        imgUrl = await _cloudinaryService.UploadImageAsync(image, Enums.CompanyImages);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Image upload failed: {ex.Message}");
                    }
                    var companyImage = new CompanyImage { File = imgUrl, CompanyId = NewCompany.CompanyId };
                    NewCompany.CompanyImages.Add(companyImage);
                }
            }
            await _context.SaveChangesAsync();

            var responseCompany = new
            {
                NewCompany.CompanyName,
                NewCompany.Address,
                NewCompany.Description,
                NewCompany.CompanyType,
                NewCompany.CompanySize,
                NewCompany.CompanyCountry,
                NewCompany.WorkingDay,
                NewCompany.OvertimePolicy,
                NewCompany.Logo,
                CompanyImages = NewCompany.CompanyImages.Select(ci => new { ci.File })
            };

            return Ok(responseCompany);
        }

        [HttpGet("accepted-company")]
        public async Task<IEnumerable<Company>> GetAcceptedCompanies()
        {
            return _context.Companies.Where(c => c.ConfirmCompany == true);
        }

        [HttpGet("pending-company")]
        public async Task<IEnumerable<Company>> GetPendingCompanies()
        {
            return _context.Companies.Where(c => c.ConfirmCompany == false);
        }

        [HttpPut("allow-company/{id}")]
        public async Task<ActionResult<Company>> AllowCompany(int id)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            //check admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                return Forbid("You do not have permission to perform this action.");
            }

            //find company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id && c.ConfirmCompany == false);
            if (company == null)
            {
                return NotFound("Company not found or be accepted.");
            }

            //set company
            company.ConfirmCompany = true;
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            return Ok(company);
        }

        [HttpDelete("reject-company/{id}")]
        public async Task<ActionResult> RejectCompany(int id)
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            // Check if user is admin
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                return Forbid("You do not have permission to perform this action.");
            }

            // 1. Find company that has not been confirmed
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null)
            {
                return NotFound("Company not found");
            }

            // Remove associated company images directly from the database
            var companyImages = await _context.CompanyImages.Where(c => c.CompanyId == company.CompanyId).ToListAsync();
            _context.CompanyImages.RemoveRange(companyImages);

            // Save changes for image deletions
            await _context.SaveChangesAsync();

            // 2. Delete the company
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted Successfully" }); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null) return NotFound(new { error = "Not Found" });
            return Ok(company);
        }

    }
}
