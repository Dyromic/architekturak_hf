using ConversionConfiguration.Models;
using ConversionConfiguration.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            return await _fileService.Get();
        }

        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFiles(string id)
        {
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
            if (file == null)
            {
                return BadRequest();
            }
            await _fileService.Add(file.FileName, file.OpenReadStream());
            return Ok();
        }

        [HttpGet("configuration")]
        public async Task<ActionResult<ConfigDto>> GetConfigutation()
        {
            return await _configService.Get();
        }

        [HttpPut("configuration")]
        public async Task<ActionResult<string>> PutConfiguration([FromBody] ConfigDto config)
        {
            return await _configService.Put(config);
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
