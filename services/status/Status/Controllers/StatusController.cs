using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Status.Models;
using Status.Services;
using System.Threading.Tasks;

namespace Status.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("[controller]")]
    [Authorize]
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
        public async Task<ActionResult<StatusEntity>> Get(string id)
        {
            return await _statusService.Get(id);
        }

        [HttpPost("/status/{id}")]
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] StatusDto status)
        {
            _logger.LogInformation("Post status: id = {}, status = {}", new object[] { id, status.status });
            await _statusService.Put(id, status.status);
            return Ok();
        }

        [HttpPost("/status/{id}/done")]
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] StatusDtoWithFileId status)
        {
            _logger.LogInformation("Post status: id = {}, status = {}, resultFileId = {}", new object[] { id, status.status, status.resultFileId});
            await _statusService.Put(id, status.status, status.resultFileId);
            return Ok();
        }
    }
}
