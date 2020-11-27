using Auth.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<string> Login(UserLogin user);
        Task<string> Register(UserRegistration user);

    }
}
