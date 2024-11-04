using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CVRecruitment.Models;
using CVRecruitment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CVRecruitment.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly UserManager<User> _userManager;
        private readonly CloudinaryService _cloudinaryService;

        public CompanyController(IConfiguration configuration, CvrecruitmentContext context, Cloudinary cloudinary, UserManager<User> userManager, CloudinaryService cloudinaryService)
        {
            _configuration = configuration;
            _context = context;
            _cloudinary = cloudinary;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _context.Companies.Include(c => c.CompanyImages).FirstOrDefaultAsync(c => c.CompanyId == id && c.ConfirmCompany == true);
            if(company == null)
            {
                return NotFound("Not found or haven't been confirm yet");
            }

            return Ok(company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AcceptCompany(int id)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            //check admin
            var isAdmin = await _userManager.IsInRoleAsync(user, Enums.RoleAdmin);
            if (!isAdmin)
            {
                return Forbid();
            }

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            if (company.ConfirmCompany == true)
            {
                return NotFound("This company has been accepted already");
            }
            company.ConfirmCompany = true;
            _context.SaveChanges();

            //Add CompanyOwner role
            var owner = await _userManager.FindByEmailAsync(company.EmailOwner);
            // Get the user's current roles
            var currentRoles = await _userManager.GetRolesAsync(owner);

            // Remove the user from all current roles
            await _userManager.RemoveFromRolesAsync(owner, currentRoles);

            // Add the user to the new role
            var result = await _userManager.AddToRoleAsync(owner, Enums.RoleCompanyOwner);

            return Ok(company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RejectCompany(int id)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            //check admin
            var isAdmin = await _userManager.IsInRoleAsync(user, Enums.RoleAdmin);
            if (!isAdmin)
            {
                return Forbid();
            }

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            var companyImages = await _context.CompanyImages.Where(i => i.CompanyId == company.CompanyId).ToListAsync();
            _context.CompanyImages.RemoveRange(companyImages);
            _context.SaveChanges();
            _context.Companies.Remove(company);
            _context.SaveChanges();
            return Ok(new { message = "deleted successfully" });
        }

        [HttpGet("accepted")]
        public async Task<IEnumerable<Company>> GetAcceptedCompanies()
        {
            return await _context.Companies.Where(c => c.ConfirmCompany == true).ToListAsync();
        }

        [HttpGet("pending")]
        public async Task<IEnumerable<Company>> GetPendingCompanies()
        {
            return await _context.Companies.Where(c => c.ConfirmCompany == false).ToListAsync();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Company>> SignUpCompany(CompanyViewModel companyViewModel)
        {
            //Check
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var existingUser = await _userManager.FindByEmailAsync(companyViewModel.EmailOwner);
            if (existingUser == null)
            {
                return BadRequest(new { message = "EmailOwner does not exist in the system." });
            }
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
                EmailCompany = companyViewModel.EmailCompany,
                EmailOwner = companyViewModel.EmailOwner,
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
                NewCompany.EmailCompany,
                NewCompany.EmailOwner,
                CompanyImages = NewCompany.CompanyImages.Select(ci => new { ci.File })
            };

            return Ok(responseCompany);
        }

    }
}
