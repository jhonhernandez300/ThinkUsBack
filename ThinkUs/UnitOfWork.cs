using ThinkUs.Interfaces;
using ThinkUs.Models;
using ThinkUs.Repositories;

namespace ThinkUs
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Employees = new Repository<Employee>(_context);
            Roles = new Repository<Role>(_context);
        }

        public IRepository<Employee> Employees { get; private set; }
        public IRepository<Role> Roles { get; private set; }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await Roles.GetAllAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
