using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThinkUs.Interfaces;
using ThinkUs.Models; // Ajusta el espacio de nombres según corresponda


namespace ThinkUs.Controllers
{
    [ApiController]
    [EnableCors("AllowOrigins")]
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IUnitOfWork unitOfWork, IEmployeeService employeeService)
        {
            _unitOfWork = unitOfWork;
            _employeeService = employeeService;
        }

        [HttpPost("UpdateEmployee")] 
        public async Task<IActionResult> UpdateEmployee(Employee employee)  
        {
            try
            {
                var (message, operationExecuted) = await _employeeService.UpdateEmployeeAsync(employee); 
                if (operationExecuted)
                {
                    return Ok(new { Message = message });
                }
                else
                {
                    return BadRequest(new { Message = message });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/employees
        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            try
            {
                var (message, operationExecuted) = await _employeeService.SaveEmployeeAsync(employee);
                if (operationExecuted)
                {
                    return Ok(new { Message = message });
                }
                else
                {
                    return BadRequest(new { Message = message });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(new { employees });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("ExportEmployeesToCsv")]
        public async Task<IActionResult> ExportEmployeesToCsv()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();

                // Genera el contenido CSV
                var csvContent = GenerateCsv(employees);

                // Nombre del archivo CSV
                var fileName = $"employees_{DateTime.Now:yyyyMMddHHmmss}.csv";

                // Devuelve el archivo CSV
                return File(new System.Text.UTF8Encoding().GetBytes(csvContent), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private string GenerateCsv(IEnumerable<EmployeeRole> employees)
        {
            var csvBuilder = new System.Text.StringBuilder();

            // Agrega el encabezado
            csvBuilder.AppendLine("Id,EmployeeName,Position,EmployeeDescription,EmployeeState,Email,RoleId,RoleName");

            // Agrega los datos de cada empleado
            foreach (var employee in employees)
            {
                csvBuilder.AppendLine($"{employee.Id},{employee.EmployeeName},{employee.Position},{employee.EmployeeDescription},{employee.EmployeeState},{employee.Email},{employee.RoleId},{employee.RoleName}");
            }

            return csvBuilder.ToString();
        }

        // GET: api/employees/5
        [HttpGet("GetEmployee/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }       

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Update properties as needed
            existingEmployee.EmployeeName = employee.EmployeeName;
            existingEmployee.Position = employee.Position;
            existingEmployee.EmployeeDescription = employee.EmployeeDescription;
            existingEmployee.EmployeeState = employee.EmployeeState;
            existingEmployee.Email = employee.Email;
            existingEmployee.EmployeePassword = employee.EmployeePassword;
            existingEmployee.RolId = employee.RolId;

            await _unitOfWork.Employees.UpdateAsync(existingEmployee);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new {message = "Employee not found with that id"});
            }

            await _unitOfWork.Employees.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Employee deleted" });
        }
    }
}
