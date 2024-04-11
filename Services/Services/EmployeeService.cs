using Databases;
using Databases.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DataContext context;

        public EmployeeService(DataContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            context.Employees.Add(employee);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddEmployeesAsync(List<Employee> employees)
        {
            context.Employees.AddRange(employees);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<Employee>> GetAllEmployeeAsync()
        {
            var employees = await context.Employees.ToListAsync();
            return employees;
        }

        public async Task<Employee> GetEmployeeByAccountNameAsync(string accountName)
        {
            var employee =  await context.Employees.FirstOrDefaultAsync(e => e.AccountName == accountName);
            return employee;
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            return employee;
        }

        public async Task<List<Employee>> GetEmployeesByDepartment(string department)
        {
            var employees =  await context.Employees.Where(x => x.Department == department).ToListAsync();
            return employees;
        }

        public void RemovePassword(Employee employee)
        {   
            if (employee != null)
            {
                employee.PasswordHash = null;
                employee.PasswordSalt = null;
            }

        }

    }
}
