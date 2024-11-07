using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CVRecruitment.Models;

public partial class Staff
{
    [Key]
    public int StaffId { get; set; }
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public int Role { get; set; }
}
