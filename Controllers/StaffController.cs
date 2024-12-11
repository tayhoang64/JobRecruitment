using System.ComponentModel.Design;
using System.Linq;
using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly UserManager<User> _userManager;

        public StaffController(IConfiguration configuration, CvrecruitmentContext context, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("checkCC")]
        public async Task<IActionResult> CheckCCWithoutCompanyId()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var getRolesStaff = await _context.Staffs.Where(s => s.UserId == user.Id).ToListAsync();
            if (getRolesStaff.Count == 0)
            {
                return Forbid();
            }
            return Ok();
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var getRolesStaff = _context.Staffs.Where(s => s.UserId == user.Id).ToList();
            if (getRolesStaff.Count == 0)
            {
                return Forbid();
            }

            var companyIds = await _context.Staffs
                .Where(s => s.UserId == user.Id)
                .GroupBy(s => s.CompanyId)
                .Select(g => g.Key)
                .ToListAsync();

            var companies = await _context.Companies.Where(c => companyIds.Contains(c.CompanyId)).ToListAsync();
            return Ok(companies);
        }

        [HttpPost]
        public async Task<ActionResult<StaffViewModel>> AddStaff(StaffViewModel staffViewModel)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == staffViewModel.CompanyId && c.ConfirmCompany == true);
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

            if(staffViewModel.RoleArray.Count < 1)
            {
                return BadRequest(new { error = "role can't be empty" });
            }

            //get staff
            var staffUser = await _userManager.FindByEmailAsync(staffViewModel.Email);

            foreach (var item in staffViewModel.RoleArray)
            {
                _context.Staffs.Add(new Staff()
                {
                    CompanyId = (int)staffViewModel.CompanyId,
                    UserId = staffUser.Id,
                    Role = item
                });
                await _context.SaveChangesAsync();
            }

            var staffs = _context.Staffs.Where(s => s.UserId == staffUser.Id && s.CompanyId == company.CompanyId).ToList();
            List<CompanyRole> roles = new List<CompanyRole>();
            foreach (var item in staffs)
            {
                roles.Add(ToCompanyRole(item));
            }

            return Ok(new StaffResponse()
            {
                User = staffUser,
                Company = company,
                Roles = roles,
            });
        }

        [HttpPut]
        public async Task<ActionResult<StaffViewModel>> UpdateRole(StaffViewModel staffViewModel)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == staffViewModel.CompanyId && c.ConfirmCompany == true);
            if (company == null)
            {
                return NotFound("company can not found or be rejected");
            }

            //check owner
            var isOwner = company.EmailOwner == user.Email;
            if (!isOwner)
            {
                return Forbid();
            }

            if (staffViewModel.RoleArray.Count < 1)
            {
                return BadRequest(new { error = "role can't be empty" });
            }

            //get staff
            var staffUser = await _userManager.FindByEmailAsync(staffViewModel.Email);

            //remove old roles
            var oldStaffs = await _context.Staffs.Where(s => s.UserId == staffUser.Id && s.CompanyId == company.CompanyId).ToListAsync();
            _context.Staffs.RemoveRange(oldStaffs);
            await _context.SaveChangesAsync();

            foreach (var item in staffViewModel.RoleArray)
            {
                _context.Staffs.Add(new Staff()
                {
                    CompanyId = (int)staffViewModel.CompanyId,
                    UserId = staffUser.Id,
                    Role = item
                });
                await _context.SaveChangesAsync();
            }

            var staffs = _context.Staffs.Where(s => s.UserId == staffUser.Id && s.CompanyId == company.CompanyId).ToList();
            List<CompanyRole> roles = new List<CompanyRole>();
            foreach (var item in staffs)
            {
                roles.Add(ToCompanyRole(item));
            }

            return Ok(new StaffResponse()
            {
                User = staffUser,
                Company = company,
                Roles = roles,
            });
        }

        [HttpDelete]
        public async Task<ActionResult<StaffViewModel>> RemoveStaff(StaffViewModel staffViewModel)
        {
            //check login
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null) return Unauthorized(new { message = "Invalid token" });

            var company = _context.Companies.FirstOrDefault(c => c.CompanyId == staffViewModel.CompanyId && c.ConfirmCompany == true);
            if (company == null)
            {
                return NotFound("company can not found or be rejected");
            }

            //check owner
            var isOwner = company.EmailOwner == user.Email;
            if (!isOwner)
            {
                return Forbid();
            }

            //get staff
            var staffUser = await _userManager.FindByEmailAsync(staffViewModel.Email);

            //remove old roles
            var oldStaffs = await _context.Staffs.Where(s => s.UserId == staffUser.Id && s.CompanyId == company.CompanyId).ToListAsync();
            _context.Staffs.RemoveRange(oldStaffs);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //---------------------------------
        private CompanyRole ToCompanyRole(Staff staff)
        {
            return new CompanyRole()
            {
                Code = staff.Role,
                Name = (staff.Role == Enums.StaffContentCreator) ? "Content Creator" : "HR"
            };
        }

        private async Task<(Models.User user, IActionResult result)> CheckStaffRole(int companyId, int staffRole)
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            var getRolesStaff = _context.Staffs.Where(s => s.CompanyId == companyId && s.UserId == user.Id).ToList();
            if (getRolesStaff.Count == 0)
            {
                return (null, Forbid());
            }
            foreach (var role in getRolesStaff) {
                if(role.Role == staffRole)
                {
                    return (user, null);
                }
            }
            return (null, Forbid());
        }

        [HttpGet("checkCC/{id}")]
        public async Task<IActionResult> IsContentCreator(int id)
        {
            var (_, result) = await CheckStaffRole(id, Enums.StaffContentCreator);
            if (result != null) {
                return result;
            }
            return Ok();
        }

        [HttpGet("checkHR/{id}")]
        public async Task<IActionResult> IsHR(int id)
        {
            var (_, result) = await CheckStaffRole(id, Enums.StaffHR);
            if (result != null)
            {
                return result;
            }
            return Ok();
        }

    }
}
