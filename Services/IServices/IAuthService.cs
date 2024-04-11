using Databases.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IAuthService
    {
        Task<Boolean> Register(RegisterDto registerDto);
        Task<String> Login(LoginDto loginDto);
    }
}
