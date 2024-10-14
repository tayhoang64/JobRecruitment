using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVRecruitment.Migrations
{
    /// <inheritdoc />
    public partial class T1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    EmailAddress = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Phone = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    PersonalLink = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Avatar = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    AboutMe = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CompanyType = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CompanySize = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CompanyCountry = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    WorkingDay = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    OvertimePolicy = table.Column<int>(type: "int", nullable: true),
                    Logo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Company__2D971CAC8BA259A9", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Salary = table.Column<double>(type: "float", nullable: true),
                    Location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    WorkStyle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    PostedDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Job__056690C2C636F866", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Skills__DFA09187C2594975", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "Template",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    File = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    UploadedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Template__F87ADD271B1B38EC", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Organization = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    IssueMonth = table.Column<int>(type: "int", nullable: true),
                    IssueYear = table.Column<int>(type: "int", nullable: true),
                    CertificateUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Certific__BBF8A7C1D0296A80", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK__Certifica__UserI__5629CD9C",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    EducationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    School = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Major = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    IsStudying = table.Column<int>(type: "int", nullable: true),
                    FromMonth = table.Column<DateTime>(type: "datetime", nullable: true),
                    FromYear = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToMonth = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToYear = table.Column<DateTime>(type: "datetime", nullable: true),
                    AdditionalDetail = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Educatio__4BBE3805208D23C4", x => x.EducationId);
                    table.ForeignKey(
                        name: "FK__Education__UserI__52593CB8",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    IsDoing = table.Column<int>(type: "int", nullable: true),
                    StartMonth = table.Column<int>(type: "int", nullable: true),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    EndMonth = table.Column<int>(type: "int", nullable: true),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    ShortDescription = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ProjectUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Project__761ABEF05448DADD", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK__Project__UserId__5535A963",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkExperience",
                columns: table => new
                {
                    WEId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Company = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    IsWorking = table.Column<int>(type: "int", nullable: true),
                    FromMonth = table.Column<int>(type: "int", nullable: true),
                    FromYear = table.Column<int>(type: "int", nullable: true),
                    ToMonth = table.Column<int>(type: "int", nullable: true),
                    ToYear = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkExpe__FA3100514870CC87", x => x.WEId);
                    table.ForeignKey(
                        name: "FK__WorkExper__UserI__534D60F1",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyImage",
                columns: table => new
                {
                    CompanyImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    File = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CompanyI__D7A1961C5A069327", x => x.CompanyImageId);
                    table.ForeignKey(
                        name: "FK__CompanyIm__Compa__5CD6CB2B",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Recruitment",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    FileCV = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Recruitm__27DEA540591D55FB", x => new { x.UserId, x.JobId });
                    table.ForeignKey(
                        name: "FK__Recruitme__JobId__5EBF139D",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "JobId");
                    table.ForeignKey(
                        name: "FK__Recruitme__UserI__571DF1D5",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobSkill__689C99DA79B69042", x => new { x.JobId, x.SkillId });
                    table.ForeignKey(
                        name: "FK__JobSkills__JobId__5DCAEF64",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "JobId");
                    table.ForeignKey(
                        name: "FK__JobSkills__Skill__5AEE82B9",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId");
                });

            migrationBuilder.CreateTable(
                name: "KeySkills",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KeySkill__406D15B471541684", x => new { x.CompanyId, x.SkillId });
                    table.ForeignKey(
                        name: "FK__KeySkills__Compa__5BE2A6F2",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK__KeySkills__Skill__59FA5E80",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId");
                });

            migrationBuilder.CreateTable(
                name: "MySkills",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MySkills__7A72C55435ED2F6E", x => new { x.UserId, x.SkillId });
                    table.ForeignKey(
                        name: "FK__MySkills__SkillI__59063A47",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId");
                    table.ForeignKey(
                        name: "FK__MySkills__UserId__5441852A",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CV",
                columns: table => new
                {
                    CVId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    File = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CV__A04CFFA3A6554FEB", x => x.CVId);
                    table.ForeignKey(
                        name: "FK__CV__TemplateId__5FB337D6",
                        column: x => x.TemplateId,
                        principalTable: "Template",
                        principalColumn: "TemplateId");
                    table.ForeignKey(
                        name: "FK__CV__UserId__5812160E",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyImage_CompanyId",
                table: "CompanyImage",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_TemplateId",
                table: "CV",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_UserId",
                table: "CV",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Education_UserId",
                table: "Education",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillId",
                table: "JobSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_KeySkills_SkillId",
                table: "KeySkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MySkills_SkillId",
                table: "MySkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId",
                table: "Project",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recruitment_JobId",
                table: "Recruitment",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperience_UserId",
                table: "WorkExperience",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "CompanyImage");

            migrationBuilder.DropTable(
                name: "CV");

            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "KeySkills");

            migrationBuilder.DropTable(
                name: "MySkills");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Recruitment");

            migrationBuilder.DropTable(
                name: "WorkExperience");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Template");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
