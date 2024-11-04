using CVRecruitment.Models;
using CVRecruitment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CvrecruitmentContext _context;

        public UserController(IConfiguration configuration, CvrecruitmentContext context)
        {
            _configuration = configuration;
            _context = context;
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

        private bool IsValidPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return phone.All(char.IsDigit) && phone.Length >= 10 && phone.Length <= 15;
        }


    }
}
