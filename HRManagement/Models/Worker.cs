using HRManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR_Management.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime WorkingStartDate { get; set; }
        public List<SalaryRecord> SalaryRecord { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
    }
}
