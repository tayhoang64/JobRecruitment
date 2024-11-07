using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CVRecruitment.Models;
using CVRecruitment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public CVController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }


        private async Task<(Models.User user, IActionResult result)> CheckUserRoleAsync()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            var isUser = await _userManager.IsInRoleAsync(user, Enums.RoleUser);
            if (!isUser)
            {
                return (null, Forbid());
            }
            return (user, null);
        }


        [HttpGet("Saved")]
        public async Task<IActionResult> GetSavedCVs()
        {
            var (user, roleCheckResult) = await CheckUserRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }
            try
            {
                ClaimsPrincipal us = HttpContext.User;
                User currentUser = await _userManager.GetUserAsync(us);

                List<Cv> cvs = await _context.Cvs
                    .Include(c => c.Template)
                    .Where(c => c.UserId == currentUser.Id)
                    .ToListAsync();

                return Ok(cvs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving saved CVs.", error = ex.Message });
            }
        }
        [HttpPost("SaveCV")]
        public async Task<IActionResult> SaveCV([FromBody] string htmlBody, int templateId)
        {
            var (user, roleCheckResult) = await CheckUserRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }
            try
            {
                ClaimsPrincipal us = HttpContext.User;
                User currentUser = await _userManager.GetUserAsync(us);

                string filename = await _cloudinaryService.SaveFileWithExtension("html", "cvs", htmlBody);
                Cv cv = new Cv
                {
                    File = filename,
                    LastUpdateAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UserId = currentUser.Id,
                    TemplateId = templateId,
                };

                _context.Cvs.Add(cv);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSavedCVs), new { id = cv.Cvid }, cv);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error saving CV", error = ex.Message });
            }
        }

        [HttpPut]


        [HttpPut("SaveEditedCV")]
        public async Task<IActionResult> SaveEditedCV([FromBody] string htmlBody, int cvId)
        {

            var (user, roleCheckResult) = await CheckUserRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }
            try
            {
                ClaimsPrincipal us = HttpContext.User;
                User currentUser = await _userManager.GetUserAsync(us);

                string filename = await _cloudinaryService.SaveFileWithExtension("html", "cvs", htmlBody);
                Cv? cv = await _context.Cvs.FirstOrDefaultAsync(c => c.Cvid == cvId && c.UserId == currentUser.Id);

                if (cv == null)
                {
                    return NotFound(new { message = "CV not found" });
                }

                cv.LastUpdateAt = DateTime.Now;
                cv.File = filename;

                await _context.SaveChangesAsync();

                return Ok(cv);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while editing the CV.", error = ex.Message });
            }
        }



        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCv(int id)
        {
            var (user, roleCheckResult) = await CheckUserRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }
            try
            {
                Cv? cv = await _context.Cvs.FirstOrDefaultAsync(c => c.Cvid == id);

                if (cv == null)
                {
                    return NotFound(new { message = "CV not found" });
                }

                _context.Cvs.Remove(cv);
                await _context.SaveChangesAsync();

                return Ok(new { message = "CV deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting CV", error = ex.Message });
            }
        }
    }
}
