using HR_Management.Models;
using HRManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HRManagement.Dtos
{
    public class WorkerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly WorkingStartDate { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
    }
}
