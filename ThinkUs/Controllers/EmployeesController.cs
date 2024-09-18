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
