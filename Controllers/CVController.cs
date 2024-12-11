using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rotativa.AspNetCore;

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
        private readonly FileService _fileService;
        private readonly IConverter _converter;

        public CVController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager, FileService fileService, IConverter converter)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _fileService = fileService;
            _converter = converter;
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

        private async Task<string> SaveCV(CVViewModel cVViewModel)
        {
            try
            {
                var fileName = $"CV_{cVViewModel.UserId}_{Guid.NewGuid()}.html";
                var filePath = Path.Combine("Public", "CVs", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await System.IO.File.WriteAllTextAsync(filePath, cVViewModel.HtmlContent);

                return $"/public/{Enums.CVs}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Error when creating html file");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCV()
        {
            var (user, result) = await CheckUserRoleAsync();
            if (result != null)
            {
                return result;
            }

            var myCVs = await _context.Cvs.Where(c => c.UserId == user.Id).ToListAsync();

            return Ok(myCVs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCV(CVViewModel cVViewModel)
        {
            var (user, result) = await CheckUserRoleAsync();
            if (result != null)
            {
                return result;
            }
            try
            {
                var fileUrl = SaveCV(cVViewModel);
                Cv cv = new Cv()
                {
                    UserId = user.Id,
                    TemplateId = cVViewModel.TemplateId,
                    CreatedAt = DateTime.Now,
                    LastUpdateAt = DateTime.Now,
                    File = fileUrl.Result,
                };
                _context.Cvs.Add(cv);
                await _context.SaveChangesAsync();
                return Ok(cv);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCV(int id, CVViewModel cVViewModel)
        {
            var (user, result) = await CheckUserRoleAsync();
            if (result != null)
            {
                return result;
            }
            var findCv = await _context.Cvs.FirstOrDefaultAsync(c => c.Cvid == id);
            if (findCv == null)
            {
                return NotFound("CV not found");
            }
            if (findCv.UserId != user.Id)
            {
                return Forbid();
            }

            //logic
            try
            {
                var resultDelete = _fileService.DeleteFile(Enums.CVs, findCv.File.Split("/")[^1]);
                if (!resultDelete)
                {
                    return StatusCode(500, "Can't delete file");
                }
                var fileUrl = SaveCV(cVViewModel);
                findCv.File = fileUrl.Result;
                findCv.LastUpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(findCv);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            var (user, result) = await CheckUserRoleAsync();
            if (result != null)
            {
                return result;
            }
            var findCv = await _context.Cvs.FirstOrDefaultAsync(c => c.Cvid == id);
            if (findCv == null)
            {
                return NotFound("CV not found");
            }
            if (findCv.UserId != user.Id)
            {
                return Forbid();
            }

            //logic
            var resultDelete = _fileService.DeleteFile(Enums.CVs, findCv.File.Split("/")[^1]);
            if (!resultDelete)
            {
                return StatusCode(500, "Can't delete file");
            }
            _context.Cvs.Remove(findCv);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (user, result) = await CheckUserRoleAsync();
            if (result != null)
            {
                return result;
            }
            var findCv = await _context.Cvs.FirstOrDefaultAsync(c => c.Cvid == id);
            if (findCv == null)
            {
                return NotFound("CV not found");
            }
            return Ok(findCv);
        }

        [HttpGet("{id}/html")]
        public async Task<IActionResult> GetHtmlContent(int id)
        {
            var cv = await _context.Cvs.FirstOrDefaultAsync(t => t.Cvid == id);
            if (cv == null)
            {
                return NotFound("CV not found");
            }
            return Ok(_fileService.ReadFileContentAsync(Enums.CVs, cv.File.Split('/').Last()).Result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var cv = await _context.Cvs.FirstOrDefaultAsync(t => t.Cvid == id);
            if (cv == null)
            {
                return NotFound("CV not found");
            }
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Document"
            };

            var objectSettings = new ObjectSettings
            {
                HtmlContent = _fileService.ReadFileContentAsync(Enums.CVs, cv.File.Split('/').Last()).Result,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "CV.pdf");
        }
    }
}
