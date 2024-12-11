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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .FirstOrDefault(j => DateTime.Now < j.EndDay && j.Status != Enums.StatusFull && j.JobId == id);
            if(job == null)
            {
                return NotFound(new {error = "job not found"});
            }
            return Ok(job);
        }

        [HttpGet]
        public async Task<IEnumerable<JobResponse>> GetAllActiveJobs()
        {
            var activeJobs = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .Where(j => DateTime.Now < j.EndDay && j.Status != Enums.StatusFull)
                .ToListAsync();

            var jobs = activeJobs.Select(job => new JobResponse
            {
                JobId = job.JobId,
                JobName = job.JobName,
                Salary = job.Salary,
                Location = job.Location,
                WorkStyle = job.WorkStyle,
                PostedDay = job.PostedDay,
                Description = job.Description,
                EndDay = job.EndDay,
                ExperienceYear = job.ExperienceYear,
                RecruitmentCount = job.RecruitmentCount,
                Status = job.Status,
                Company = new Company
                {
                    CompanyId = job.Company.CompanyId,
                    CompanyName = job.Company.CompanyName,
                    Address = job.Company.Address,
                    Description = job.Company.Description,
                    CompanyType = job.Company.CompanyType,
                    CompanySize = job.Company.CompanySize,
                    CompanyCountry = job.Company.CompanyCountry,
                    WorkingDay = job.Company.WorkingDay,
                    OvertimePolicy = job.Company.OvertimePolicy,
                    Logo = job.Company.Logo,
                    ConfirmCompany = job.Company.ConfirmCompany,
                },
                Skills = job.Skills.Select(skill => new Skill
                {
                    SkillId = skill.SkillId,
                    SkillName = skill.SkillName,
                }).ToList()
            });

            return jobs;
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

        [HttpGet("company/{id}")]
        public async Task<IActionResult> GetJobsByCompanyId(int id)
        {
            var activeJobs = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .Where(j => DateTime.Now < j.EndDay && j.Status != Enums.StatusFull && j.CompanyId == id)
                .ToListAsync();

            var jobs = activeJobs.Select(job => new JobResponse
            {
                JobId = job.JobId,
                JobName = job.JobName,
                Salary = job.Salary,
                Location = job.Location,
                WorkStyle = job.WorkStyle,
                PostedDay = job.PostedDay,
                Description = job.Description,
                EndDay = job.EndDay,
                ExperienceYear = job.ExperienceYear,
                RecruitmentCount = job.RecruitmentCount,
                Status = job.Status,
                Company = new Company
                {
                    CompanyId = job.Company.CompanyId,
                    CompanyName = job.Company.CompanyName,
                    Address = job.Company.Address,
                    Description = job.Company.Description,
                    CompanyType = job.Company.CompanyType,
                    CompanySize = job.Company.CompanySize,
                    CompanyCountry = job.Company.CompanyCountry,
                    WorkingDay = job.Company.WorkingDay,
                    OvertimePolicy = job.Company.OvertimePolicy,
                    Logo = job.Company.Logo,
                    ConfirmCompany = job.Company.ConfirmCompany,
                },
                Skills = job.Skills.Select(skill => new Skill
                {
                    SkillId = skill.SkillId,
                    SkillName = skill.SkillName,
                }).ToList()
            });

            return Ok(jobs);
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

        [HttpGet("RecommendJob/{jobId}")]
        public async Task<IEnumerable<JobResponse>> RecommendJob(int jobId)
        {
            var currentJob = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.JobId == jobId);

            if (currentJob == null)
            {
                return Enumerable.Empty<JobResponse>();
            }
            decimal? ParseSalary(string? salary)
            {
                if (decimal.TryParse(salary, out var parsedSalary))
                {
                    return parsedSalary;
                }
                return null;
            }
            var currentJobSalary = ParseSalary(currentJob.Salary);
            var recommendedJobs = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .Where(j =>
                    j.JobId != jobId && 
                    DateTime.Now < j.EndDay &&
                    j.Status != Enums.StatusFull &&
                    j.Location == currentJob.Location &&
                    j.ExperienceYear == currentJob.ExperienceYear)
                .ToListAsync();
            recommendedJobs = recommendedJobs.Where(j =>
            {
                var jobSalary = ParseSalary(j.Salary);
                return !jobSalary.HasValue || (currentJobSalary.HasValue && Math.Abs(jobSalary.Value - currentJobSalary.Value) <= 5000);
            }).Take(3).ToList();
            var jobs = recommendedJobs.Select(job => new JobResponse
            {
                JobId = job.JobId,
                JobName = job.JobName,
                Salary = job.Salary,
                Location = job.Location,
                WorkStyle = job.WorkStyle,
                PostedDay = job.PostedDay,
                Description = job.Description,
                EndDay = job.EndDay,
                ExperienceYear = job.ExperienceYear,
                RecruitmentCount = job.RecruitmentCount,
                Status = job.Status,
                Company = new Company
                {
                    CompanyId = job.Company.CompanyId,
                    CompanyName = job.Company.CompanyName,
                    Address = job.Company.Address,
                    Description = job.Company.Description,
                    CompanyType = job.Company.CompanyType,
                    CompanySize = job.Company.CompanySize,
                    CompanyCountry = job.Company.CompanyCountry,
                    WorkingDay = job.Company.WorkingDay,
                    OvertimePolicy = job.Company.OvertimePolicy,
                    Logo = job.Company.Logo,
                    ConfirmCompany = job.Company.ConfirmCompany,
                },
                Skills = job.Skills.Select(skill => new Skill
                {
                    SkillId = skill.SkillId,
                    SkillName = skill.SkillName,
                }).ToList()
            });

            return jobs;
        }



    }
}