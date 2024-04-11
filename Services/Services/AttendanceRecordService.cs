using Databases;
using Databases.DTOs;
using Databases.Entities;
using Databases.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AttendanceRecordService : IAttendanceRecordService
    {
        private readonly DataContext context;
        private readonly IEmployeeService employeeService;
        private readonly ILogger<AttendanceRecordService> logger;

        public AttendanceRecordService(DataContext context, IEmployeeService employeeService, ILogger<AttendanceRecordService> logger)
        {
            this.context = context;
            this.employeeService = employeeService;
            this.logger = logger;
        }

        public async Task<bool> AddAttendanceRecord(string employeeId, AttendanceRecordType type)
        {
            try
            {
                var currentRecord = await GetRecordByEmployeeIdAndDate(employeeId, DateTime.Now);
                if (currentRecord == null)
                {
                    var newRecord = new AttendanceRecord();
                    newRecord.Id = Guid.NewGuid().ToString();
                    newRecord.EmployeeId = employeeId;
                    newRecord.Date = DateTime.Now.Date;

                    this.SetRecordTime(newRecord, type, DateTime.Now);
                    context.AttendanceRecords.Add(newRecord);
                    return await context.SaveChangesAsync() > 0;
                }
                else
                {
                    this.SetRecordTime(currentRecord, type, DateTime.Now);
                    context.AttendanceRecords.Update(currentRecord);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
            

        }
        private void SetRecordTime(AttendanceRecord record, AttendanceRecordType type, DateTime time)
        {
            if (type == AttendanceRecordType.Arrival)
            {
                record.ArrivalTime = DateTime.Now;
            }
            else
            {
                record.LeaveTime = DateTime.Now;
            }
        }

        public async Task<AttendanceRecord?> GetRecordByEmployeeIdAndDate(string employeeId, DateTime date)
        {
            var result = await context.AttendanceRecords.Where(x => x.EmployeeId == employeeId && x.Date.Date == date.Date).FirstOrDefaultAsync();
            return result != null ? result : null;
        }

        public async Task<List<AttendanceRecord>> GetRecordsByIdEmployee(string employeeId)
        {
            var records = await context.AttendanceRecords.Where(x => x.EmployeeId == employeeId).ToListAsync();
            return records;
        }

        public async Task<List<ManagedAttendanceRecordResult>> GetManagedAttendanceRecords(string managedId)
        {
            var manager = await employeeService.GetEmployeeByIdAsync(managedId);
            if (manager.Extension is ManagerExtension managerExtension)
            {
                Task<List<Employee>> employeeTask;
                Task<List<AttendanceRecord>> recordTask;

                if (managerExtension.ManagerType == ManagerType.DepartmentManager)
                {
                    employeeTask = employeeService.GetEmployeesByDepartment(manager.Department);
                    recordTask = this.GetRecordsByDepartment(manager.Department);
                }
                else
                {
                    employeeTask = employeeService.GetAllEmployeeAsync();
                    recordTask = this.GetAllRecords();
                }
                await Task.WhenAll(employeeTask, recordTask);

                var employees = employeeTask.Result;
                var records = recordTask.Result;

                var result = new List<ManagedAttendanceRecordResult>();

                if (records.Any())
                {
                    var recordGroups = records.GroupBy(i => i.EmployeeId);
                    foreach (var group in recordGroups)
                    {
                        var employee = employees.FirstOrDefault(i => String.Equals(i.Id, group.Key));
                        if (employee != null)
                        {
                            var item = new ManagedAttendanceRecordResult();
                            item.Employee = employee;
                            item.AttendanceRecords = group.OrderByDescending(i => i.Date).ToList();
                            result.Add(item);
                        }
                        else
                        {
                            this.logger.LogWarning($"Cannot find employee with id: {group.Key}");
                        }
                    }
                }
                return result;
            }
            throw new UnauthorizedAccessException("You are not allowed!!");
        }

        public async Task<List<AttendanceRecord>> GetAllRecords()
        {
            return await context.AttendanceRecords.ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetRecordsByDepartment(string department)
        {
            var employee = await employeeService.GetEmployeesByDepartment(department);
            return await context.AttendanceRecords.Where(x => employee.Select(x => x.Id).ToList().Contains(x.EmployeeId)).ToListAsync();
        }
    }
}
