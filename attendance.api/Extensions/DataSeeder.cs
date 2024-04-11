using Databases;
using Databases.DTOs;
using Databases.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace attendance.api.Extensions
{
    public class DataSeeder
    {
        public static void SeedData(DataContext context)
        {
            try
            {
                if (context.Employees.Any() || context.AttendanceRecords.Any()) return;
                var directory = FindSolutionPath();
                var employeesJsonPath = Path.Combine(directory, "Databases/employees.json");
                var recordsJsonPath = Path.Combine(directory, "Databases/records.json");
                //Seed employees
                var employeesJson = File.ReadAllText(employeesJsonPath);
                var employees = JsonSerializer.Deserialize<List<RegisterDto>>(employeesJson);
                if (employees is null || !employees.Any()) return;

                var resultEmployees = new List<Employee>();

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
                context.Employees.AddRange(resultEmployees);
                context.SaveChanges();

                // Seed records
                var recordsJson = File.ReadAllText(recordsJsonPath);
                var records = JsonSerializer.Deserialize<List<AttendanceRecord>>(recordsJson);
                if (records is null || !records.Any()) return;

                context.AttendanceRecords.AddRange(records);

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
           
        }


        private static string FindSolutionPath()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;

            while (currentPath != null)
            {
                string[] solutionFiles = Directory.GetFiles(currentPath, "*.sln");
                if (solutionFiles.Length > 0)
                {
                    return currentPath;
                }

                currentPath = Directory.GetParent(currentPath)?.FullName;
            }

            return null;
        }

    }
}
