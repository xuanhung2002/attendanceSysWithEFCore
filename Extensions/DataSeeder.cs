using Databases;
using Databases.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extensions
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
                // Seed employees
                //var employeesJson = File.ReadAllText(employeesJsonPath);
                //var employees = JsonSerializer.Deserialize<List<Employee>>(employeesJson);
                //if (employees is null || !employees.Any()) return;

                //var passwordBytes = Encoding.UTF8.GetBytes("password");
                //foreach (var employee in employees)
                //{
                //    using var hashFunc = new HMACSHA256();
                //    employee.PasswordHash = hashFunc.ComputeHash(passwordBytes);
                //    employee.PasswordSalt = hashFunc.Key;
                //}
                //context.Employees.AddRange(employees);

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
