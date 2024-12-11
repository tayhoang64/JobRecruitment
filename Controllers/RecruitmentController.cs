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
    public class RecruitmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;
        private readonly FileService _fileService;

        public RecruitmentController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager, FileService fileService)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _fileService = fileService;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var recruitment = _context.Recruitments.FirstOrDefault(r => r.UserId == user.Id && r.JobId == id);
            if (recruitment == null) {
                return NotFound("Recruitment not found");
            }
            return Ok(recruitment);
        }

        [HttpDelete("{jobId}")]
        public async Task<IActionResult> CancelApply(int jobId)
        {
            //check
            var (user, result) = await CheckLogin();
            if (result != null)
            {
                return result;
            }
            var recruitment = _context.Recruitments.FirstOrDefault(r => r.JobId == jobId && r.UserId == user.Id);
            if(recruitment == null)
            {
                return NotFound("recruitment not found");
            }
            var job = _context.Jobs.FirstOrDefault(j => j.JobId == recruitment.JobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }
            var checkApply = _context.Recruitments.FirstOrDefault(j => j.UserId == user.Id && j.JobId == recruitment.JobId);
            if (checkApply == null)
            {
                return BadRequest(new
                {
                    error = "You haven't sent any application for this job before"
                });
            }
            //logic
            _context.Recruitments.RemoveRange(checkApply);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Apply(RecruitmentViewModel recruitmentViewModel)
        {
            //check
            var (user, result) = await CheckLogin();
            if(result != null)
            {
                return result;
            }
            var job = _context.Jobs.FirstOrDefault(j => j.JobId == recruitmentViewModel.JobId);
            if(job == null)
            {
                return NotFound("Job not found");
            }
            var checkApply = _context.Recruitments.FirstOrDefault(j => j.UserId == user.Id && j.JobId == recruitmentViewModel.JobId);
            if (checkApply != null) {
                return BadRequest(new
                {
                    error = "You already sent an application for this job"
                });
            }
            //logic
            if (recruitmentViewModel.FormFile == null || recruitmentViewModel.FormFile.ContentType != "application/pdf")
            {
                return BadRequest(new
                {
                    error = "Only PDF files are allowed for the CV upload."
                });
            }
            string cvLink;
            try
            {
                cvLink = await _fileService.UploadHtmlAsync(recruitmentViewModel.FormFile, Enums.CVs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }
            Recruitment recruitment = new Recruitment()
            {
                UserId = user.Id,
                JobId = recruitmentViewModel.JobId,
                FileCv = cvLink,
                SentAt = DateTime.Now,
            };
            _context.Add(recruitment);
            await _context.SaveChangesAsync();
            return Ok(new RecruitmentResponse()
            {
                UserId = recruitment.UserId,
                JobId = recruitment.JobId,
                FileCV = recruitment.FileCv,
                SentAt= recruitment.SentAt,
            });
        }
    }
}
