using attendance.api.Extensions;
using Databases.DTOs;
using Databases.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace attendance.api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AddEmployee([FromBody] RegisterDto employee)
        {
            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(employee.Password);
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                Type = employee.Type,
                Role = employee.Role,
                AccountName = employee.AccountName,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Sex = employee.Sex,
                Department = employee.Department,
                PhoneNumber = employee.PhoneNumber,
                IsIntern = employee.IsIntern,
                Extension = Converter.Converter.ConvertToEmployeeExtensionObject(employee.Type, employee.Extension.ToString()),
                PasswordHash = hashFunc.ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key
            };
            var result = await employeeService.AddEmployeeAsync(newEmployee);
            return result ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            var result = await employeeService.GetEmployeeByIdAsync(id);
            if (result == null)
            {
                return BadRequest("user not found");
            }
            else
            {
                var employeeDto = Converter.Converter.ConvertEmployeeToEmployeeDto(result);
                return Ok(employeeDto);
            }
        }

        [HttpGet("accountname/{accountName}")]
        [Authorize(Policy = "RequireAdminRoleOrManagerRole")]
       public async Task<IActionResult> GetEmployeeByAccountName(String accountName)
        {
            var result = await employeeService.GetEmployeeByAccountNameAsync(accountName);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(Converter.Converter.ConvertEmployeeToEmployeeDto(result));
        }



        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllEmployee()
        {
            var result = await employeeService.GetAllEmployeeAsync();
            if (result == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(result.Select(e => Converter.Converter.ConvertEmployeeToEmployeeDto(e)));
            }
        }

        [HttpPost("import")]
        [FileUploadOperation.FileContentType]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> ImportImployees([FromForm(Name = "uploadedFile")] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                var resultEmployees = new List<Employee>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var jsonString = await reader.ReadToEndAsync();
                    var employees = JsonConvert.DeserializeObject<List<RegisterDto>>(jsonString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                  
                    foreach (var employee in employees)
                    {
                        using var hashFunc = new HMACSHA256();
                        var passwordBytes = Encoding.UTF8.GetBytes(employee.Password);
                        var newEmployee = new Employee
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = employee.Type,
                            Role = employee.Role,
                            AccountName = employee.AccountName,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Sex = employee.Sex,
                            Department = employee.Department,
                            PhoneNumber = employee.PhoneNumber,
                            IsIntern = employee.IsIntern,
                            Extension = Converter.Converter.ConvertToEmployeeExtensionObject(employee.Type, employee.Extension.ToString()),
                            PasswordHash = hashFunc.ComputeHash(passwordBytes),
                            PasswordSalt = hashFunc.Key
                        };
                        resultEmployees.Add(newEmployee);
                    }
                }
                var result = await employeeService.AddEmployeesAsync(resultEmployees);
                return result ? Ok("Import succesfully") : BadRequest("Faill");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
