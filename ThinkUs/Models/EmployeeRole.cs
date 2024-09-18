namespace ThinkUs.Models
{
    public class EmployeeRole
    {
        public int Id { get; set; }
        public required string EmployeeName { get; set; }
        public required string Position { get; set; }
        public required string EmployeeDescription { get; set; }
        public required bool EmployeeState { get; set; }
        public required string Email { get; set; }
        public required string EmployeePassword { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
