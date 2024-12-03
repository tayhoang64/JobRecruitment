using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly CvrecruitmentContext _context;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole<int>> roleManager, CvrecruitmentContext context, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        private async Task<(Models.User user, IActionResult result)> CheckAdmin()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, Enums.RoleAdmin);
            if (!isAdmin)
            {
                return (null, Forbid());
            }
            return (user, null);
        }

        [HttpGet]
        public async Task<IEnumerable<RoleResponse>> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            List<RoleResponse> rolesResponse = new List<RoleResponse>();
            foreach (var role in roles) {
                rolesResponse.Add(new RoleResponse()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                });
            }
            return rolesResponse;
        }

        [HttpPut]
        public async Task<IActionResult> ChangeRole(RoleViewModel roleViewModel)
        {
            var (user, resultCheckAdmin) = await CheckAdmin();
            if (resultCheckAdmin != null)
            {
                return resultCheckAdmin;
            }

            var findUser = await _userManager.FindByIdAsync(roleViewModel.UserId.ToString());
            if (findUser == null)
            {
                return NotFound("User not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(findUser);

            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(findUser, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest("Failed to remove old roles.");
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(findUser, roleViewModel.RoleName);
            if (!addRoleResult.Succeeded)
            {
                return BadRequest("Failed to add new role.");
            }

            return Ok("Role updated successfully.");
        }


    }
}
