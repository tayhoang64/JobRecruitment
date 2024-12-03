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
    public class MySkillController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public MySkillController(IConfiguration configuration, CvrecruitmentContext context, FileService fileService, UserManager<User> userManager)
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

        [HttpGet]
        public async Task<IActionResult> GetMySkills()
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var skills = _context.MySkills.Include(s => s.Skill).Where(c => c.UserId == user.Id);
            List<MySkillResponse> response = new List<MySkillResponse>();
            foreach (var item in skills)
            {
                response.Add(new MySkillResponse()
                {
                    SkillId = item.SkillId,
                    SkillName = item.Skill.SkillName,
                    Level = item.Level,
                });
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMySKill(int id , ViewModels.MySkillViewModel mySkill)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }

            if(user.Id != id)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var oldMySkills = _context.MySkills.Where(s=> s.UserId == user.Id).ToList();
            _context.MySkills.RemoveRange(oldMySkills);
            await _context.SaveChangesAsync();

            foreach (var item in mySkill.SkillIds)
            {
                _context.MySkills.Add(new MySkill()
                {
                    UserId = user.Id,
                    SkillId = item.SkillId,
                    Level = item.Level,
                });
                await _context.SaveChangesAsync();
            }
            return Ok("updated successfully");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMySKill(ViewModels.MySkillViewModel mySkill)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (var item in mySkill.SkillIds)
            {
                _context.MySkills.Add(new MySkill()
                {
                    UserId = user.Id,
                    SkillId = item.SkillId,
                    Level = item.Level,
                });
                await _context.SaveChangesAsync();
            }
            return Ok("posted successfully");
        }

    }
}
