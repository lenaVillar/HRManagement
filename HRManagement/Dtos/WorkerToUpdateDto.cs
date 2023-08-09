using System.ComponentModel.DataAnnotations;

namespace HRManagement.Dtos
{
    public class WorkerToUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime WorkingStartDate { get; set; }
        public List<int> RolesId { get; set; } = new();
        public float WorkingStartSalary { get; set; }
    }
}
