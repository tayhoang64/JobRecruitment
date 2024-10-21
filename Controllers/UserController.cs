using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

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

    }
}
