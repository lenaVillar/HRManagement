using System.ComponentModel.DataAnnotations;

namespace HRManagement.Dtos
{
    public class SalaryRecordrDto
    {

        public double Salary { get; set; }
        [DataType(DataType.Date)]
        public DateTime IncreaseSalaryDate { get; set; }
    }
}
