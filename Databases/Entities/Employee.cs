using Databases.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Entities
{
    public class Employee
    {
        [Key]
        public string Id { get; set; }
        [EnumDataType(typeof(EmployeeType))]
        public EmployeeType Type { get; set; }

        [EnumDataType(typeof(Roles))]
        public Roles Role { get; set; }
        public string AccountName { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [EnumDataType(typeof(Sex))]
        public Sex Sex { get; set; }
        public string Department { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsIntern { get; set; }


        public string? ExtensionJson { get; set; }

        [NotMapped]
        public EmployeeExtension? Extension
        {
            get
            {
                if (string.IsNullOrEmpty(ExtensionJson))
                {
                    return null;
                }

                switch (Type)
                {
                    case EmployeeType.Developer:
                        return JsonConvert.DeserializeObject<DeveloperExtension>(ExtensionJson, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                    case EmployeeType.QualityAssurance:
                        return JsonConvert.DeserializeObject<QualityAssuranceExtension>(ExtensionJson, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                        });
                    case EmployeeType.Manager:
                        return JsonConvert.DeserializeObject<ManagerExtension>(ExtensionJson, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                    default:
                        throw new ArgumentException("Invalid role type");
                }
            }
            set => ExtensionJson = JsonConvert.SerializeObject(value);
        }

    }
}
