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
    public class TemplateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public TemplateController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        private async Task<(Models.User user, IActionResult result)> CheckCvDecoratorRoleAsync()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return (null, Unauthorized(new { message = "Invalid token" }));
            }
            var isCvDecorator = await _userManager.IsInRoleAsync(user, Enums.RoleCVDecorator);
            if (!isCvDecorator)
            {
                return (null, Forbid());
            }
            return (user, null);
        }


        [HttpGet]
        public async Task<IEnumerable<Template>> GetAll()
        {
            return await _context.Templates.ToListAsync();
        }

        [HttpPost]

        public async Task<IActionResult> CreateTemplate([FromForm] TemplateViewModel templateViewModel, IFormFile file)
        {
            var (user, roleCheckResult) = await CheckCvDecoratorRoleAsync();
            if (roleCheckResult != null) 
            {
                return roleCheckResult;
            }

            var template = new Template
            {
                Title = templateViewModel.Title,
                UploadedBy = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            if (file != null)
            {
                var fileUrl = await _cloudinaryService.UploadHtmlAsync(file, Enums.Templates);
                template.File = fileUrl;
            }

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = template.TemplateId }, template);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromForm] TemplateViewModel templateViewModel, IFormFile? file)
        {
            var (user, roleCheckResult) = await CheckCvDecoratorRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }


            var template = await _context.Templates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            if (template.UploadedBy != user.Id) 
            {
                return Forbid();
            }

            template.Title = templateViewModel.Title;
            template.LastUpdatedAt = DateTime.UtcNow;

            if (file != null)
            {
                if (!string.IsNullOrEmpty(template.File))
                {
                    await _cloudinaryService.DeleteImageAsync(template.File);
                }

                var fileUrl = await _cloudinaryService.UploadHtmlAsync(file, Enums.Templates);
                template.File = fileUrl;
            }

            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var (user, roleCheckResult) = await CheckCvDecoratorRoleAsync();
            if (roleCheckResult != null)
            {
                return roleCheckResult;
            }
            var template = await _context.Templates.FindAsync(id);
            if (template.UploadedBy != user.Id)
            {
                return Forbid();
            }
            if (template == null)
            {
                return NotFound(new { message = "Template not found." });
            }
            var publicId = GetPublicIdFromUrl(template.File); 
            await _cloudinaryService.DeleteImageAsync(publicId);
            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments;
            return segments[segments.Length - 1].Split('.')[0];
        }
    }

    
}
