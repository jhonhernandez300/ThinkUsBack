using System.Data;

namespace ThinkUs.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public required string EmployeeName { get; set; }
        public required string Position { get; set; }
        public required string EmployeeDescription { get; set; }
        public required bool EmployeeState { get; set; }
        public required string Email { get; set; }
        public required string EmployeePassword { get; set; }
        public int? RoleId { get; set; }        
    }
}
