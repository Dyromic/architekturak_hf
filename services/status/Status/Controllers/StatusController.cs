using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Status.Models;
using Status.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Status.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;
        private readonly StatusService _statusService;

        public StatusController(ILogger<StatusController> logger, StatusService statusService)
        {
            _logger = logger;
            _statusService = statusService;
        }

        [HttpGet("/status/{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            return await _statusService.Get(id);
        }

        [HttpPost("/status/{id}")]
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] StatusDto status)
        {
            await _statusService.Put(id, status.status);
            return Ok();
        }
    }
}
