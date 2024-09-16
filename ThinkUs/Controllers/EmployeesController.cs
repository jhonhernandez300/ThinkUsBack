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

        public EmployeesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/employees
        [HttpGet("GetEmployees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return Ok(employees);
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

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            return BadRequest(ModelState);
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
            existingEmployee.RoleId = employee.RoleId;

            await _unitOfWork.Employees.UpdateAsync(existingEmployee);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await _unitOfWork.Employees.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
