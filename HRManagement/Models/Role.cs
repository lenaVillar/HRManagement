using HRManagement.Models;
using Newtonsoft.Json;

namespace HR_Management.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Worker> Workers { get; set; } = new();
    }

}
