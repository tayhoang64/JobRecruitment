using CVRecruitment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CVRecruitment.Services
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JwtMiddleware(RequestDelegate next, IConfiguration config, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _config = config;
            _serviceScopeFactory = serviceScopeFactory; 
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, token); // Make it async
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token) // Change to async
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
                var userId = userIdClaim != null ? userIdClaim.Value : null;

                // Create a new scope to access the DbContext
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CvrecruitmentContext>();

                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

                    if (user != null)
                    {
                        context.Items["User"] = user;
                    }
                }
            }
            catch(Exception e) 
            {
                // Handle token validation failure (e.g., log the error)
                await Console.Out.WriteLineAsync(e.Message);
            }
        }
    }


}
