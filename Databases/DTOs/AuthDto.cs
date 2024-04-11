﻿using Databases.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.DTOs
{
    public class LoginDto
    {
        public string AccountName { get; set; } = null!;

        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }

    public class RegisterDto
    {
        public EmployeeType Type { get; set; }
        public Roles Role { get; set; }
        public string AccountName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Sex Sex { get; set; }
        public string Department { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsIntern { get; set; }
        public object? Extension { get; set; } = null!;
    }
}
