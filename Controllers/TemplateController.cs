using CloudinaryDotNet.Actions;
using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
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
        private readonly FileService _fileService;

        public TemplateController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager, FileService fileService)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _fileService = fileService;
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
        public async Task<IActionResult> GetAll()
        {
            var templates = await _context.Templates.ToListAsync();
            List<TemplateResponse> response = new List<TemplateResponse>();
            foreach (var item in templates)
            {
                response.Add(new TemplateResponse() { 
                    TemplateId = item.TemplateId,
                    Title = item.Title,
                    File = item.File,
                    CreatedAt = item.CreatedAt,
                    UploadedBy = item.UploadedBy,
                    LastUpdatedAt = item.LastUpdatedAt,
                    Image = item.Image,
                    User = await _context.Users.FirstOrDefaultAsync(u => u.Id == item.UploadedBy)
                });

            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if(template == null)
            {
                return NotFound("Template not found");
            }
            return Ok(template);
        }

        [HttpGet("{id}/html")]
        public async Task<IActionResult> GetHtmlContent(int id)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (template == null)
            {
                return NotFound("Template not found");
            }
            return Ok(_fileService.ReadFileContentAsync(Enums.Templates, template.File.Split('/').Last()).Result);
        }

        [HttpPost]

        public async Task<IActionResult> CreateTemplate([FromForm] TemplateViewModel templateViewModel, IFormFile file, IFormFile image)
        {
            var (user, roleCheckResult) = await CheckCvDecoratorRoleAsync();
            if (roleCheckResult != null) 
            {
                return roleCheckResult;
            }
            if (file == null || !file.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) || file.ContentType != "text/html")
            {
                return BadRequest("The file must be a valid HTML file.");
            }
            if (string.IsNullOrEmpty(templateViewModel.Title))
            {
                return BadRequest("Title is required");
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
                var fileUrl = await _fileService.UploadHtmlAsync(file, Enums.Templates);
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return BadRequest("File upload failed. Please try again.");
                }
                template.File = fileUrl;
            }

            if (image != null)
            {
                string imageUrl;
                try
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image, Enums.Templates);
                }
                catch (Exception ex) {
                    return BadRequest("Image upload failed. Please try again.");
                }
                template.Image = imageUrl;
            }

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "created template successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromForm] TemplateViewModel templateViewModel, IFormFile? file, IFormFile? image)
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
            if (string.IsNullOrEmpty(templateViewModel.Title))
            {
                return BadRequest("Title is required");
            }
            template.Title = templateViewModel.Title;
            template.LastUpdatedAt = DateTime.UtcNow;

            if (file != null)
            {
                if (!file.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) || file.ContentType != "text/html")
                {
                    return BadRequest("The file must be a valid HTML file.");
                }
                if (!string.IsNullOrEmpty(template.File))
                {
                    _fileService.DeleteFile(Enums.Templates, template.File.Split("/")[^1]);
                }
                var fileUrl = await _fileService.UploadHtmlAsync(file, Enums.Templates);
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return BadRequest("File upload failed. Please try again.");
                }
                template.File = fileUrl;
            }
            if (image != null)
            {
                string imageUrl;
                try
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image, Enums.Templates);
                }
                catch (Exception ex)
                {
                    return BadRequest("Image upload failed. Please try again.");
                }
                template.Image = imageUrl;
            }

            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "updated template successfully"
            });
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

            var cvs = _context.Cvs.Where(c => c.TemplateId == template.TemplateId).ToList();
            _context.Cvs.RemoveRange(cvs);
            await _context.SaveChangesAsync();

            var result = _fileService.DeleteFile(Enums.Templates, template.File.Split("/")[^1]);
            if (!result)
            {
                return StatusCode(500, "Can't delete file");
            }
            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "deleted template successfully"
            });
        }
    }

    
}
