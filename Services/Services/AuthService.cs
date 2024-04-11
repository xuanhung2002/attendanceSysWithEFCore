using Databases.DTOs;
using Databases.Entities;
using Databases.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeService employeeService;
        private readonly ITokenService tokenService;

        public AuthService(IEmployeeService employeeService, ITokenService tokenService)
        {
            this.employeeService = employeeService;
            this.tokenService = tokenService;
        }
        public async Task<string> Login(LoginDto loginDto)
        {
            loginDto.AccountName = loginDto.AccountName.ToLower();
            var existedEmployee = await employeeService.GetEmployeeByAccountNameAsync(loginDto.AccountName);
            if (existedEmployee is null) return null;
            using var hashFunc = new HMACSHA256(existedEmployee.PasswordSalt);
            var passwordBytes = Encoding.UTF8.GetBytes(loginDto.Password);
            var passwordHash = hashFunc.ComputeHash(passwordBytes);
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != existedEmployee.PasswordHash[i])
                    return null;
            }
            return tokenService.GenerateToken(existedEmployee);
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {

            registerDto.AccountName = registerDto.AccountName.ToLower();
            if (await employeeService.GetEmployeeByAccountNameAsync(registerDto.AccountName) is not null)
                return false;

            using var hashFunc = new HMACSHA256();
            var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);

            var newEmployee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                Type = registerDto.Type,
                Role = registerDto.Role,
                AccountName = registerDto.AccountName,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Sex = registerDto.Sex,
                Department = registerDto.Department,
                PhoneNumber = registerDto.PhoneNumber,
                IsIntern = registerDto.IsIntern,
                Extension = Converter.Converter.ConvertToEmployeeExtensionObject(registerDto.Type, registerDto.Extension.ToString()),
                PasswordHash = hashFunc.ComputeHash(passwordBytes),
                PasswordSalt = hashFunc.Key
            };         
            await employeeService.AddEmployeeAsync(newEmployee);
            return true;
        }
    }
}
