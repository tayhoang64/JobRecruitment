using CVRecruitment.Models;
using Microsoft.EntityFrameworkCore;
using CVRecruitment.Controllers;
using CVRecruitment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CVRecruitment.Services;
using Microsoft.AspNetCore.Authentication;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Add services to the container.
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<CvrecruitmentContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var cloudinaryAccount = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);

var cloudinary = new CloudinaryDotNet.Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<CvrecruitmentContext>()
.AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()  
            .AllowAnyMethod()   
            .AllowAnyHeader()); 
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    googleOptions.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
})
.AddCookie()
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    facebookOptions.CallbackPath = "/signin-facebook";
});


var app = builder.Build();

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CvrecruitmentContext>();

    if (!context.Roles.Any())
    {
        context.Roles.Add(new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString() });
        context.Roles.Add(new IdentityRole<int> { Name = "CVDecorator", NormalizedName = "CVDECORATOR", ConcurrencyStamp = Guid.NewGuid().ToString() });
        context.Roles.Add(new IdentityRole<int> { Name = "Treasurer", NormalizedName = "Treasurer".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        context.Roles.Add(new IdentityRole<int> { Name = "CompanyOwner", NormalizedName = "CompanyOwner".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        context.Roles.Add(new IdentityRole<int> { Name = "User", NormalizedName = "USER", ConcurrencyStamp = Guid.NewGuid().ToString() });
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

//var configurationValues = builder.Configuration.GetDebugView();
//var audience = builder.Configuration["JwtSettings:Audience"];

app.MapControllers();

app.Run();


