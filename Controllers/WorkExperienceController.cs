using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkExperienceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public WorkExperienceController(IConfiguration configuration, CvrecruitmentContext context, FileService fileService, UserManager<User> userManager)
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


        // Create a workExperience
        [HttpPost]
        public async Task<IActionResult> CreateWorkExperience(ViewModels.WorkExperienceViewModel workExperience)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }

            if (workExperience.IsWorking != 0 && workExperience.IsWorking != 1)
            {
                return BadRequest("Invalid value IsWorking !!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkExperience newWorkExperience = new WorkExperience()
            {
                JobTitle = workExperience.JobTitle,
                Company = workExperience?.Company,
                FromMonth = workExperience?.FromMonth,
                FromYear = workExperience?.FromYear,    
                IsWorking = workExperience?.IsWorking,
                Description = workExperience?.Description,
                ToMonth = workExperience?.ToMonth,
                ToYear = workExperience?.ToYear,
                UserId = user.Id,
            };

            _context.WorkExperiences.Add(newWorkExperience);
            await _context.SaveChangesAsync();

            return Ok(newWorkExperience);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWorkExperience()
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            return Ok(_context.WorkExperiences.Where(cc => cc.UserId == user.Id));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var WorkExperience = _context.WorkExperiences.FirstOrDefault(WorkExperience => WorkExperience.Weid == id);
            if (WorkExperience == null)
            {
                return NotFound("workExperiences not found");
            }
            if (WorkExperience.UserId != user.Id)
            {
                return Forbid();
            }
            _context.WorkExperiences.Remove(WorkExperience);
            await _context.SaveChangesAsync();
            return Ok("Finish!!");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkExperience(int id, WorkExperienceViewModel UpdateWorkExperience)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var workExperience = _context.WorkExperiences.FirstOrDefault(workExperience => workExperience.Weid == id);
            if (workExperience == null)
            {
                return NotFound("workExperiences not found");
            }
            if (workExperience.UserId != user.Id)
            {
                return Forbid();
            }
            workExperience.JobTitle = UpdateWorkExperience.JobTitle;
            workExperience.Company = UpdateWorkExperience.Company;
            workExperience.FromMonth = UpdateWorkExperience.FromMonth;
            workExperience.ToMonth = UpdateWorkExperience.ToMonth;
            workExperience.FromYear = UpdateWorkExperience.FromYear;
            workExperience.ToYear = UpdateWorkExperience.ToYear;
            workExperience.Description = UpdateWorkExperience.Description;
            workExperience.IsWorking = UpdateWorkExperience.IsWorking;
            await _context.SaveChangesAsync();
            return Ok("Updated succesfully");
        }
    }
}
