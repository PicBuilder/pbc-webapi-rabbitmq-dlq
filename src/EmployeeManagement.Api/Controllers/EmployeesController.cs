using Employee.EventBus.Data.Interfaces;
using Employee.EventBus.Dtos;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IPublishEndpoint _publish;

        public EmployeesController(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostEmployee(EmployeeDto employeeDto)
        {
          if (employeeDto == null)
          {
              return Problem("Entity set 'EmployeeDto'  is null.");
          }
            await _publish.Publish<EmployeeDto>(employeeDto);

            return Ok(Results.Accepted());
        }
    }
}
