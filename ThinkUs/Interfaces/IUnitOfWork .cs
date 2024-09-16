using ThinkUs.Models;

namespace ThinkUs.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Employee> Employees { get; }
        IRepository<Role> Roles { get; }
        Task<int> CompleteAsync();
    }
}
