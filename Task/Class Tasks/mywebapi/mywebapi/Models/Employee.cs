using mywebapi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace mywebapi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("dept")]
        public int DeptId { get; set; }
        public Department dept { get; set; }

    }
}
