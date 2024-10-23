using CVRecruitment.Models;
using CVRecruitment.Services;
using CVRecruitment.ViewModels;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CVRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly CvrecruitmentContext _context;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService, CvrecruitmentContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailAddress = model.Email,
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token }, Request.Scheme);
            await _emailService.SendEmailAsync(model.Email, "Email Confirmation", $"Please confirm your email by clicking on this link: <a href='{confirmationLink}'>Confirm Email</a>");

            return Ok("User registered successfully. Please check your email to confirm.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized("Email has not been confirmed yet. Please confirm your email.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                _context.SaveChanges();
                await _userManager.AddToRoleAsync(user, "User");
                _context.SaveChanges();
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Email confirmation failed.");
        }

        /*[HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            string redirectUrl = Url.Action("GoogleLoginCallback", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Unauthorized("Error with Google login.");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var picture = info.Principal.FindFirstValue("urn:google:picture");

            var emailValidationResult = await ValidateEmail(email);
            if (emailValidationResult != null)
            {
                return emailValidationResult;
            }

            var user = new Models.User()
            {
                Email = email,
                EmailAddress = email,
                UserName = name,
                Avatar = picture,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            await _userManager.AddToRoleAsync(user, "User");
            _context.SaveChanges();

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }
        */

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
            if (payload == null)
            {
                return Unauthorized("Invalid Google token.");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new Models.User()
                {
                    UserName = payload.Email,
                    NormalizedEmail = payload.Email.ToUpper(),
                    Email = payload.Email,
                    NormalizedUserName = payload.Email.ToUpper(),
                    Avatar = payload.Picture,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create user.");
                }
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }


        /* [HttpGet("login-facebook")]
         public IActionResult LoginWithFacebook()
         {
             string redirectUrl = Url.Action("FacebookLoginCallback", "Auth");
             var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
             return Challenge(properties, "Facebook");
         }

         [HttpGet("facebook-login-callback")]
         public async Task<IActionResult> FacebookLoginCallback()
         {

             var info = await _signInManager.GetExternalLoginInfoAsync();
             if (info == null)
             {
                 return Unauthorized("Error with Facebook login.");
             }

             var email = info.Principal.FindFirstValue(ClaimTypes.Email);
             var name = info.Principal.FindFirstValue(ClaimTypes.Name);
             string userId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
             string pictureUrl = $"https://graph.facebook.com/{userId}/picture?type=large";
             var emailValidationResult = await ValidateEmail(email);
             if (emailValidationResult != null)
             {
                 return emailValidationResult;
             }

             var user = new Models.User()
             {
                 Email = email,
                 EmailAddress = email,
                 UserName = name,
                 Avatar = pictureUrl,
                 SecurityStamp = Guid.NewGuid().ToString(),
                 EmailConfirmed = true
         };
             _context.Users.Add(user);
             _context.SaveChanges();
             await _userManager.AddToRoleAsync(user, "User");
             _context.SaveChanges();

             return Ok(new
             {
                 Email = email,
                 Name = name,
                 PictureUrl = pictureUrl
             });
         }*/

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginRequest request)
        {
            // Xác thực token Facebook
            var fbTokenValidationUrl = $"https://graph.facebook.com/me?access_token={request.Token}&fields=email,name,picture";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(fbTokenValidationUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized("Invalid Facebook token.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var fbUser = JsonConvert.DeserializeObject<FacebookUser>(json);
                var user = await _userManager.FindByEmailAsync(fbUser.Email);
                if (user == null)
                {
                    user = new Models.User
                    {
                        UserName = fbUser.Email,
                        NormalizedEmail = fbUser.Email.ToUpper(),
                        Email = fbUser.Email,
                        NormalizedUserName = fbUser.Email.ToUpper(),
                        Avatar = fbUser.Picture.Data.Url,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    await _userManager.CreateAsync(user);
                }
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
        }


        private async Task<IActionResult> ValidateEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
            {
                return BadRequest("Invalid email format. Please provide a valid email.");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Conflict("Email already exists. Please use a different email.");
            }

            return null;
        }

        private string GenerateJwtToken(Models.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }




    }


}
