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
            var validationResult = ValidateCompanyViewModel(companyViewModel);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return BadRequest(validationResult);
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
                NewCompany.EmailCompany,
                NewCompany.EmailOwner,
                NewCompany.Logo,
                CompanyImages = NewCompany.CompanyImages.Select(ci => new { ci.File })
            };

            return Ok(responseCompany);
        }

        

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Company>> UpdateCompany(int id, CompanyViewModel companyViewModel)
        {
            //Check
            var validationResult = ValidateCompanyViewModel(companyViewModel);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return BadRequest(validationResult);
            }
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == id && c.ConfirmCompany == true);
            if(company == null)
            {
                return NotFound("company can not found or be rejected");
            }

            //check owner
            var isOwner = company.EmailOwner == user.Email;
            if (!isOwner)
            {
                return Forbid();
            }
            //Update
            company.CompanyName = companyViewModel.CompanyName;
            company.Address = companyViewModel.Address;
            company.Description = companyViewModel.Description;
            company.CompanyType = companyViewModel.CompanyType;
            company.CompanySize = companyViewModel.CompanySize;
            company.CompanyCountry = companyViewModel.CompanyCountry;
            company.WorkingDay = companyViewModel.WorkingDay;
            company.OvertimePolicy = companyViewModel.OvertimePolicy;
            company.EmailCompany = companyViewModel.EmailCompany;
            company.EmailOwner = companyViewModel.EmailOwner;
            if (companyViewModel.Logo != null)
            {
                try
                {
                    company.Logo = await _cloudinaryService.UploadImageAsync(companyViewModel.Logo, Enums.Avatars);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

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
                    var companyImage = new CompanyImage { File = imgUrl, CompanyId = company.CompanyId };
                    company.CompanyImages.Add(companyImage);
                }
            }
            await _context.SaveChangesAsync();

            var responseCompany = new
            {
                company.CompanyName,
                company.Address,
                company.Description,
                company.CompanyType,
                company.CompanySize,
                company.CompanyCountry,
                company.WorkingDay,
                company.OvertimePolicy,
                company.Logo,
                company.EmailCompany,
                company.EmailOwner,
                CompanyImages = company.CompanyImages.Select(ci => new { ci.File })
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

            var owner = _userManager.FindByEmailAsync(company.EmailOwner);
            if (owner == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var currentRoles = await _userManager.GetRolesAsync(owner.Result);

            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(owner.Result, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest("Failed to remove old roles.");
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(owner.Result, "CompanyOwner");
            if (!addRoleResult.Succeeded)
            {
                return BadRequest("Failed to add new role.");
            }

            return Ok(company);
        }

        [HttpDelete("reject-company/{id}")]
        public async Task<ActionResult> RejectCompany(int id)
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                return Forbid("You do not have permission to perform this action.");
            }

            var company = await _context.Companies
                .Include(c => c.Jobs)
                    .ThenInclude(j => j.Skills)      
                .Include(c => c.Jobs)
                    .ThenInclude(j => j.Recruitments) 
                .Include(c => c.CompanyImages)     
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return NotFound("Company not found");
            }

            foreach (var job in company.Jobs)
            {
                var jobSkills = await _context.Set<Dictionary<string, object>>("JobSkill")
                    .Where(js => EF.Property<int>(js, "JobId") == job.JobId)
                    .ToListAsync();
                _context.Set<Dictionary<string, object>>("JobSkill").RemoveRange(jobSkills);

                _context.Recruitments.RemoveRange(job.Recruitments);
            }

            _context.Jobs.RemoveRange(company.Jobs);

            _context.CompanyImages.RemoveRange(company.CompanyImages);

            await _context.SaveChangesAsync();

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted Successfully" });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.Include(c => c.CompanyImages).FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null) return NotFound(new { error = "Not Found" });
            return Ok(company);
        }

        [HttpGet("get-own-companies")]
        public async Task<IActionResult> GetOwnCompanies()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var isInRole = await _userManager.IsInRoleAsync(user, "CompanyOwner");
            if (!isInRole)
            {
                return Forbid("You do not have permission to perform this action.");
            }
            return Ok(_context.Companies.Where(c => c.EmailOwner == user.Email).ToList());
        }



        private string ValidateCompanyViewModel(CompanyViewModel companyViewModel)
        {
            if (companyViewModel == null)
            {
                return "Company information is required.";
            }

            if (string.IsNullOrEmpty(companyViewModel.CompanyName))
            {
                return "CompanyName is required.";
            }

            if (string.IsNullOrEmpty(companyViewModel.Address))
            {
                return "Address is required.";
            }

            if (string.IsNullOrEmpty(companyViewModel.EmailCompany) || !IsValidEmail(companyViewModel.EmailCompany))
            {
                return "A valid EmailCompany is required.";
            }

            if (string.IsNullOrEmpty(companyViewModel.EmailOwner) || !IsValidEmail(companyViewModel.EmailOwner))
            {
                return "A valid EmailOwner is required.";
            }

            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
