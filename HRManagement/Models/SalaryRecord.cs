using HR_Management.Models;
using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class SalaryRecord
    {
        public int Id { get; set; }  
        public double Salary { get; set; }
        [DataType(DataType.Date)]
        public DateTime IncreaseSalaryDate { get; set; }
        public Worker Worker { get; set; } = new();
        
    }
}
