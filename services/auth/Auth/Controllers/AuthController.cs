using Auth.DTO;
using Auth.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Controllers
{

    [ApiController]
    [Route("auth")]
    [EnableCors("CorsPolicy")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _loginService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IAuthService loginService)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<String>> Login([FromBody] UserLogin user )
        {
            string token = await _loginService.Login(user);
            if (token != null)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult<String>> Register([FromBody] UserRegistration user)
        {
            string token = await _loginService.Register(user);
            if (token != null)
            {
                return Ok(new { Token = token });
            }
            return BadRequest();

        }

    }
}
