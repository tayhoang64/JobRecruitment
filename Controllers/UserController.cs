using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<User> _userManager;

        public UserController(IConfiguration configuration, CvrecruitmentContext context, CloudinaryService cloudinaryService, UserManager<User> userManager)
        {
            _configuration = configuration;
            _context = context;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok(user);
        }

        [HttpPut("update")]
        public IActionResult UpdateInfo(UpdateInfoModel updateInfoModel)
        {
            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            string? resultCheckUserInfo = CheckUserInfo(updateInfoModel);
            if (resultCheckUserInfo != null) {
                return BadRequest(resultCheckUserInfo);
            }

            var findUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            findUser.FullName = updateInfoModel.FullName;
            findUser.Title = updateInfoModel.Title;
            findUser.Phone = updateInfoModel.Phone;
            findUser.Gender = updateInfoModel?.Gender;
            findUser.City = updateInfoModel.City;
            findUser.Address = updateInfoModel.Address;
            findUser.PersonalLink = updateInfoModel.PersonalLink;
            findUser.AboutMe = updateInfoModel.AboutMe;
            _context.SaveChanges();

            return Ok(findUser);
        }

        //-------------------------------------------------------
        private string? CheckUserInfo(UpdateInfoModel model)
        {
            model.FullName = string.IsNullOrWhiteSpace(model.FullName) ? null : model.FullName;
            model.Title = string.IsNullOrWhiteSpace(model.Title) ? null : model.Title;
            model.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone;
            model.City = string.IsNullOrWhiteSpace(model.City) ? null : model.City;
            model.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address;
            model.PersonalLink = string.IsNullOrWhiteSpace(model.PersonalLink) ? null : model.PersonalLink;
            model.AboutMe = string.IsNullOrWhiteSpace(model.AboutMe) ? null : model.AboutMe;

            if (!string.IsNullOrWhiteSpace(model.Phone) && !IsValidPhoneNumber(model.Phone))
            {
                return "Invalid phone number";
            }

            if (model.DateOfBirth != null && model.DateOfBirth > DateTime.Now)
            {
                return "Date of birth cannot be later than the current date.";
            }

            if (model.Gender != 0 && model.Gender != 1)
            {
                return "invalid gender";
            }

            return null;
        }

        [HttpPut("uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                return BadRequest("Please select an image file.");
            }

            var user = (Models.User)HttpContext.Items["User"];
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins.Any(login => login.LoginProvider == "Facebook" || login.LoginProvider == "Google"))
            {
                return Forbid("You cannot change the avatar when logged in with Facebook or Google.");
            }
            string avatarUrl;
            try
            {
                avatarUrl = await _cloudinaryService.UploadImageAsync(avatarFile, Enums.Avatars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }

            var trackedUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
            if (trackedUser != null)
            {
                _context.Entry(trackedUser).State = EntityState.Detached;
            }
            user.Avatar = avatarUrl;
            _context.Attach(user);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, "Failed to update user avatar.");
            }

            return Ok(new { AvatarUrl = avatarUrl });
        }

        private bool IsValidPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return phone.All(char.IsDigit) && phone.Length >= 10 && phone.Length <= 15;
        }


    }
}
