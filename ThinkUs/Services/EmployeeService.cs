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

        public async Task<(string Message, bool IsValid)> ValidateEmployeeCredentialsAsync(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[dbo].[ValidateEmployeeCredentials]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Añadir los parámetros del SP
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@EmployeePassword", password);

                    // Parámetros de salida
                    var messageParam = new SqlParameter("@message", SqlDbType.VarChar, -1)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(messageParam);

                    var isValidParam = new SqlParameter("@isValid", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(isValidParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Obtener los valores de los parámetros de salida
                    var message = (string)messageParam.Value;
                    var isValid = (bool)isValidParam.Value;

                    return (message, isValid);
                }
            }
        }


        public async Task<IEnumerable<EmployeeRole>> GetAllEmployeesAsync()
        {
            var employees = new List<EmployeeRole>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[dbo].[GetAllEmployees]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new EmployeeRole
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                                Position = reader.GetString(reader.GetOrdinal("Position")),
                                EmployeeDescription = reader.GetString(reader.GetOrdinal("EmployeeDescription")),
                                EmployeeState = reader.GetBoolean(reader.GetOrdinal("EmployeeState")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                EmployeePassword = reader.GetString(reader.GetOrdinal("EmployeePassword")),
                                RoleId = reader.IsDBNull(reader.GetOrdinal("RolId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RolId")),
                                RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                            });
                        }
                    }
                }
            }

            return employees;
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
