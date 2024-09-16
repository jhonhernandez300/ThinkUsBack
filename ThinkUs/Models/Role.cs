namespace ThinkUs.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string RoleName { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
