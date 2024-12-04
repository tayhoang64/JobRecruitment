

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
    public class ProjectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;

        public ProjectController(IConfiguration configuration, CvrecruitmentContext context, FileService fileService, UserManager<User> userManager)
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

        [HttpPost]
        public async Task<IActionResult> CreateProject(ViewModels.ProjectViewModel project)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }

            if(project.IsDoing != 0 && project.IsDoing !=1) {
                return BadRequest("Invalid value IS DOING !!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Project newProject = new Project()
            {
                ProjectName = project.ProjectName,
                IsDoing = project.IsDoing,
                ProjectUrl = project.ProjectUrl,
                StartMonth =project.StartMonth,
                StartYear = project.StartYear,
                EndMonth = project.EndMonth,
                EndYear = project.EndYear,
                ShortDescription = project.ShortDescription,
                UserId = user.Id,
            };

            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();
            return Ok(newProject);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProject()
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            return Ok(_context.Projects.Where(c => c.UserId == user.Id));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }

            var proj = _context.Projects.FirstOrDefault(proj => proj.ProjectId == id);
            if (proj == null)
            {
                return NotFound("Project not found");
            }
            if (proj.UserId != user.Id)
            {
                return Forbid();
            }
            _context.Projects.Remove(proj);
            await _context.SaveChangesAsync();
            return Ok("Finish!!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjects(int id, ProjectViewModel UpdateProject)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            if (UpdateProject.IsDoing != 0 && UpdateProject.IsDoing != 1)
            {
                return BadRequest("Invalid value IS DOING !!");
            }
            var project = _context.Projects.FirstOrDefault(project => project.ProjectId == id);
            if (project == null)
            {
                return NotFound("projects not found");
            }
            if (project.UserId != user.Id)
            {
                return Forbid();
            }
            project.ProjectName = UpdateProject.ProjectName;
            project.IsDoing = UpdateProject.IsDoing;
            project.ProjectUrl = UpdateProject.ProjectUrl;
            project.EndMonth = UpdateProject.EndMonth;
            project.EndYear = UpdateProject.EndYear;
            project.StartMonth = UpdateProject.StartMonth;
            project.StartYear = UpdateProject.StartYear;
            await _context.SaveChangesAsync();
            return Ok("Updated succesfully");
        }

    }
}
