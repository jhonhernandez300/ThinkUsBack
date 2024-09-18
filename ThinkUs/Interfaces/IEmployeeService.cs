﻿using ThinkUs.Models;

namespace ThinkUs.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeRole>> GetAllEmployeesAsync();
        Task<(string Message, bool OperationExecuted)> UpdateEmployeeAsync(Employee employee);  
        Task<(string Message, bool OperationExecuted)> SaveEmployeeAsync(Employee employee);
    }
}
