using Databases.DTOs;
using Databases.Entities;
using Databases.Models;
using Newtonsoft.Json;

namespace Converter
{
    public class Converter
    {
        public static EmployeeExtension ConvertToEmployeeExtensionObject(EmployeeType type, string value)
        {
            switch (type)
            {
                case EmployeeType.Developer:
                    return JsonConvert.DeserializeObject<DeveloperExtension>(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                case EmployeeType.QualityAssurance:
                    return JsonConvert.DeserializeObject<QualityAssuranceExtension>(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                case EmployeeType.Manager:
                    return JsonConvert.DeserializeObject<ManagerExtension>(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                default:
                    return null!;
            }
        }

        public static EmployeeDto ConvertEmployeeToEmployeeDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                AccountName = employee.AccountName,
                Department = employee.Department,
                IsIntern = employee.IsIntern,
                Role = employee.Role,
                Type = employee.Type,
                Sex = employee.Sex,
                PhoneNumber = employee.PhoneNumber,
                Extension = employee.Extension,
            };
        }
    }
}
