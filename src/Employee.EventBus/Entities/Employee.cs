using System.ComponentModel.DataAnnotations;

namespace Employee.EventBus.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
