using Databases.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IEmployeeService
    {

        Task<List<Employee>> GetAllEmployeeAsync();
        Task<Employee> GetEmployeeByIdAsync(String id);
        Task<Employee> GetEmployeeByAccountNameAsync(String accountName);

        Task<Boolean> AddEmployeeAsync(Employee employee);
        Task<Boolean> AddEmployeesAsync(List<Employee> employees);

        Task<List<Employee>> GetEmployeesByDepartment(string department);
    }
}
