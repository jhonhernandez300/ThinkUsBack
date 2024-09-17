using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ThinkUs.Interfaces;
using ThinkUs.Models;

namespace ThinkUs.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<(string Message, bool OperationExecuted)> UpdateEmployeeAsync(Employee employee)  
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[dbo].[UpdateEmployee]", connection))  
                {
                    command.CommandType = CommandType.StoredProcedure;
                   
                    command.Parameters.AddWithValue("@employeeId", employee.Id);  
                    command.Parameters.AddWithValue("@employeeName", employee.EmployeeName);
                    command.Parameters.AddWithValue("@position", employee.Position);
                    command.Parameters.AddWithValue("@employeeDescription", employee.EmployeeDescription);
                    command.Parameters.AddWithValue("@employeeState", employee.EmployeeState);
                    command.Parameters.AddWithValue("@email", employee.Email);
                    command.Parameters.AddWithValue("@employeePassword", employee.EmployeePassword);
                    command.Parameters.AddWithValue("@rolId", employee.RolId);

                    // Adding output parameters
                    var messageParam = new SqlParameter("@message", SqlDbType.VarChar, -1)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParam);

                    var operationExecutedParam = new SqlParameter("@operationExecuted", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(operationExecutedParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Retrieve output parameter values
                    string message = (string)command.Parameters["@message"].Value;
                    bool operationExecuted = (bool)command.Parameters["@operationExecuted"].Value;

                    if (operationExecuted == false)
                    {
                        throw new Exception(message);
                    }
                    return (message, operationExecuted);
                }
            }
        }

        public async Task<(string Message, bool OperationExecuted)> SaveEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[dbo].[SaveEmployee]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Adding input parameters
                    command.Parameters.AddWithValue("@employeeName", employee.EmployeeName);
                    command.Parameters.AddWithValue("@position", employee.Position);
                    command.Parameters.AddWithValue("@employeeDescription", employee.EmployeeDescription);
                    command.Parameters.AddWithValue("@employeeState", employee.EmployeeState);
                    command.Parameters.AddWithValue("@email", employee.Email);
                    command.Parameters.AddWithValue("@employeePassword", employee.EmployeePassword);
                    command.Parameters.AddWithValue("@rolId", employee.RolId);

                    // Adding output parameters
                    var messageParam = new SqlParameter("@message", SqlDbType.VarChar, -1)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParam);

                    var operationExecutedParam = new SqlParameter("@operationExecuted", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(operationExecutedParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Retrieve output parameter values
                    string message = (string)command.Parameters["@message"].Value;
                    bool operationExecuted = (bool)command.Parameters["@operationExecuted"].Value;
                    
                    if (operationExecuted == false)
                    {
                        throw new Exception(message);
                    }
                    return (message, operationExecuted);
                }
            }
        }
    }
}
