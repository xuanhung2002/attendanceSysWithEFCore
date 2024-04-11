using Databases.DTOs;
using Databases.Entities;
using Databases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IAttendanceRecordService
    {
        Task<List<AttendanceRecord>> GetRecordsByIdEmployee(string employeeId);
        Task<bool> AddAttendanceRecord(string employeeId, AttendanceRecordType type);

        Task<AttendanceRecord> GetRecordByEmployeeIdAndDate(string employeeId, DateTime date);

        Task<List<ManagedAttendanceRecordResult>> GetManagedAttendanceRecords(string managedId);

        Task<List<AttendanceRecord>> GetAllRecords();

        Task<List<AttendanceRecord>> GetRecordsByDepartment(string department);

    }
}
