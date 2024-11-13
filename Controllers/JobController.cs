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
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public JobController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        private async Task<(Models.User user, IActionResult result)> IsHRorContentCreator(int CompanyId, int target)
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            var staff = _context.Staffs.Where(s => s.UserId == user.Id && s.CompanyId == CompanyId).ToList();
            foreach (var s in staff)
            {
                if(s.Role == target)
                {
                    return (user, null);
                }
            }
            return (null, Forbid());
        }

        [HttpGet]
        public async Task<IEnumerable<Job>> GetAllActiveJobs()
        {
            return await _context.Jobs.Include(j => j.Company).Include(j => j.Skills).Where(j => DateTime.Now < j.EndDay && j.Status != Enums.StatusFull).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostJob(JobViewModel jobViewModel)
        {
            //check
            var (user, check) = await IsHRorContentCreator((int)jobViewModel.CompanyId, Enums.StaffContentCreator);
            if(check != null)
            {
                return check;
            }
            var listSkill = new List<Skill>();
            foreach (var item in _context.Skills)
            {
                if (jobViewModel.SkillIds.Contains(item.SkillId))
                {
                    listSkill.Add(item);
                }
            }
            //logic
            Job job = new Job()
            {
                JobName = jobViewModel.JobName,
                Salary = jobViewModel.Salary,
                Location = jobViewModel.Location,
                WorkStyle = jobViewModel.WorkStyle,
                PostedDay = DateTime.Now,
                Description = jobViewModel.Description,
                EndDay = jobViewModel.EndDay,
                RecruitmentCount = jobViewModel.RecruitmentCount,
                ExperienceYear = jobViewModel.ExperienceYear,
                Status = false,
                CompanyId = (int)jobViewModel.CompanyId,
                UserId = user.Id,
                Skills = listSkill
            };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return Ok(job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, JobViewModel jobViewModel)
        {
            //check
            var (user, check) = await IsHRorContentCreator((int)jobViewModel.CompanyId, Enums.StaffContentCreator);
            if (check != null)
            {
                return check;
            }
            var listSkill = new List<Skill>();
            foreach (var item in _context.Skills)
            {
                if (jobViewModel.SkillIds.Contains(item.SkillId))
                {
                    listSkill.Add(item);
                }
            }
            //owner check
            var findJob = _context.Jobs.Include(j => j.Skills).FirstOrDefault(j => j.JobId == id && DateTime.Now < j.EndDay && j.Status != Enums.StatusFull);
            if (findJob == null)
            {
                return NotFound("Job not found");
            }
            if(findJob.UserId != user.Id)
            {
                return Forbid();
            }
            //logic
            findJob.JobName = jobViewModel.JobName;
            findJob.Salary = jobViewModel.Salary;
            findJob.Location = jobViewModel.Location;
            findJob.WorkStyle = jobViewModel.WorkStyle;
            findJob.Description = jobViewModel.Description;
            findJob.EndDay = jobViewModel.EndDay;
            findJob.RecruitmentCount = jobViewModel.RecruitmentCount;
            findJob.ExperienceYear = jobViewModel.ExperienceYear;
            findJob.Skills = listSkill;
            await _context.SaveChangesAsync();
            return Ok(findJob);
        }

        [HttpPut("set-full/{id}")]
        public async Task<IActionResult> SetStatusFull(int id, JobViewModel jobViewModel)
        {
            //check
            var (user, check) = await IsHRorContentCreator((int)jobViewModel.CompanyId, Enums.StaffContentCreator);
            if (check != null)
            {
                return check;
            }
            var listSkill = new List<Skill>();
            foreach (var item in _context.Skills)
            {
                if (jobViewModel.SkillIds.Contains(item.SkillId))
                {
                    listSkill.Add(item);
                }
            }
            //owner check
            var findJob = _context.Jobs.Include(j => j.Skills).FirstOrDefault(j => j.JobId == id && DateTime.Now < j.EndDay && j.Status != Enums.StatusFull);
            if (findJob == null)
            {
                return NotFound("Job not found");
            }
            if (findJob.UserId != user.Id)
            {
                return Forbid();
            }
            //logic
            findJob.Status = Enums.StatusFull;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id, JobViewModel jobViewModel)
        {
            //check
            var (user, check) = await IsHRorContentCreator((int)jobViewModel.CompanyId, Enums.StaffContentCreator);
            if (check != null)
            {
                return check;
            }
            var listSkill = new List<Skill>();
            foreach (var item in _context.Skills)
            {
                if (jobViewModel.SkillIds.Contains(item.SkillId))
                {
                    listSkill.Add(item);
                }
            }
            //owner check
            var findJob = _context.Jobs.Include(j => j.Skills).FirstOrDefault(j => j.JobId == id && DateTime.Now < j.EndDay && j.Status != Enums.StatusFull);
            if (findJob == null)
            {
                return NotFound("Job not found");
            }
            if (findJob.UserId != user.Id)
            {
                return Forbid();
            }
            //logic
            _context.Jobs.Remove(findJob);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Name, Salary, Location, Skill, WokExp
        [HttpGet("search")]
        public async Task<IEnumerable<Job>> SearchJobs(
                                    string? jobName = null,
                                    string? location = null,
                                    string? salary = null,
                                    int workExp = 0,
                                    [FromQuery] List<int> skillIds = null)
        {
            var query = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .Where(j => DateTime.Now < j.EndDay && j.Status != Enums.StatusFull)
                .AsQueryable();

            if (!string.IsNullOrEmpty(jobName))
            {
                query = query.Where(j => j.JobName.Contains(jobName));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(j => j.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(salary))
            {
                query = query.Where(j => j.Salary.Contains(salary));
            }

            if (workExp > 0)
            {
                query = query.Where(j => j.ExperienceYear >= workExp);
            }

            if (skillIds != null && skillIds.Any())
            {
                query = query.Where(j => j.Skills.Any(s => skillIds.Contains(s.SkillId)));
            }

            return await query.ToListAsync();
        }


    }
}