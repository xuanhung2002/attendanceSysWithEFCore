using Databases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using System.Security.Claims;

namespace attendance.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceRecordController : ControllerBase
    {
        private readonly IAttendanceRecordService attendanceRecordService;

        public AttendanceRecordController(IAttendanceRecordService attendanceRecordService)
        {
            this.attendanceRecordService = attendanceRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceRecordOfCurrentEmployee()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var records = await attendanceRecordService.GetRecordsByIdEmployee(userId);
            if(records == null)
            {
                return NoContent();
            }
            return Ok(records);
        }


        [HttpPost("records/{employeeId}/{type}")]
        public async Task<IActionResult> AddAttendanceRecord([FromRoute] String employeeId, [FromRoute] AttendanceRecordType type)
        {
            var result = await attendanceRecordService.AddAttendanceRecord(employeeId, type);
            return result ? Ok("Add success") : BadRequest(result);
        }

        [HttpGet("records/{managerId}/managed")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> GetManagedAttendanceRecords([FromRoute] string managerId)
        {
            try
            {
                var result = await this.attendanceRecordService.GetManagedAttendanceRecords(managerId);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("you are not allow");
            }          
        }

        [HttpPost("import")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> ImportAttendanceRecords([FromForm] IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
