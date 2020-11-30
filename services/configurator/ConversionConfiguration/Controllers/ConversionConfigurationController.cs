using ConversionConfiguration.Models;
using ConversionConfiguration.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConversionConfiguration.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ConversionConfigurationController : ControllerBase
    {
        private readonly ILogger<ConversionConfigurationController> _logger;
        private readonly FileService _fileService;
        private readonly ConfigService _configService;
        private readonly PropertySettings _propertySettings;

        public ConversionConfigurationController(ILogger<ConversionConfigurationController> logger,
            FileService fileService, ConfigService configService, PropertySettings propertySettings)
        {
            _logger = logger;
            _fileService = fileService;
            _configService = configService;
            _propertySettings = propertySettings;
        }

        [HttpGet("files")]
        public async Task<ActionResult<List<FileDto>>> GetFiles()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await _fileService.Get(userId);
        }

        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFiles(string id)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Stream stream = null;
            try
            {
                stream = await _fileService.GetStream(id);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not open file: {}\n{}", new object[] { id, e });
            }
            if (stream == null)
                return NotFound();
            return File(stream, "application/octet-stream", await _fileService.GetName(id));
        }

        [HttpPost("files")]
        public async Task<IActionResult> PostFiles([FromForm] IFormFile file)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (file == null)
            {
                return BadRequest();
            }
            var id = await _fileService.Add(file.FileName, userId, file.OpenReadStream());
            return Ok(new {Id = id});
        }

        [HttpGet("configs")]
        public IActionResult GetConfigutation()
        {

            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(new { Configs = _configService.Get(userId) });
        }

        [HttpPut("configs")]
        public async Task<ActionResult<string>> PutConfiguration([FromBody] ConfigDto config)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await _configService.Put(config, userId);
        }

        [HttpPost("start/{id}")]
        public async Task<IActionResult> PostStart(string id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(
                    new Dictionary<string, string>{ { _propertySettings.StatusProp, "Queued" } }),
                    Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(
                    string.Format(_propertySettings.StatusEndpoint, id), content);
                _logger.LogDebug("Response: {}", result);
            }
            catch (Exception e) {
                _logger.LogWarning("Status not sent: {}", e.Message);
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(
                    string.Format(_propertySettings.SvgEndpoint, id), null);
                _logger.LogInformation("Response: {}", result);
                _logger.LogInformation("Content: {}", await result.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                _logger.LogError("Start not sent: " + e.Message);
            }
            return Ok();
        }
    }
}
