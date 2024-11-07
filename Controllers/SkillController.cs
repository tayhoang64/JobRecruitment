using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<SkillController> _logger;

        public SkillController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager, ILogger<SkillController> logger)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<IActionResult> CheckAdminRoleAsync()
        {
            var user = HttpContext.Items["User"] as Models.User;
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, Enums.RoleAdmin);
            if (!isAdmin)
            {
                return Forbid();
            }
            return null;
        }

        [HttpGet]
        public async Task<IEnumerable<Skill>> GetAll()
        {
            return await _context.Skills.ToListAsync();
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSkills([FromForm] SkillViewModel model)
        {
            var roleCheckResult = await CheckAdminRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }

            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {
                    await model.File.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var skillName = worksheet.Cells[row, 2].Text.Trim();

                            if (!string.IsNullOrEmpty(skillName))
                            {
                                var existingSkill = await _context.Skills
                                    .FirstOrDefaultAsync(s => s.SkillName == skillName);

                                if (existingSkill == null)
                                {
                                    var newSkill = new Skill
                                    {
                                        SkillName = skillName
                                    };
                                    _context.Skills.Add(newSkill);
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Ok("Skills uploaded successfully.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error uploading skills");
                return BadRequest($"Error uploading skills: {ex.Message}");
            }
        }


        [HttpPut("UpdateSkill/{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] Skill updatedSkill)
        {
            var roleCheckResult = await CheckAdminRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }

            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound("Skill not found.");
            }

            skill.SkillName = updatedSkill.SkillName;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(skill);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating skill");
                return BadRequest($"Error updating skill: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSkill/{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var roleCheckResult = await CheckAdminRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }

            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound("Skill not found.");
            }

            _context.Skills.Remove(skill);
            try
            {
                await _context.SaveChangesAsync();
                return Ok("Skill deleted successfully.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting skill");
                return BadRequest($"Error deleting skill: {ex.Message}");
            }
        }
    }
}
