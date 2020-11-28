using ConversionConfiguration.Models;
using ConversionConfiguration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConversionConfiguration.Controllers
{
    [ApiController]
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
        public async Task<ActionResult<string>> PutConfiguration(ConfigDto config)
        {
            return await _configService.Put(config);
        }

        [HttpPost("start/{id}")]
        public async Task<IActionResult> PostStart(string id)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(
                    new Dictionary<string, string>{ { _propertySettings.StatusProp, "Queued" } }),
                    Encoding.UTF8, "application/json");
                var result = await new HttpClient().PostAsync(
                    string.Format(_propertySettings.StatusEndpoint, id), content);
                _logger.LogInformation("Response: {}", result);
            }
            catch (Exception e) {
                _logger.LogWarning("Status not sent: {}", e.Message);
            }

            try
            {
                var result = await new HttpClient().PostAsync(string.Format(_propertySettings.StatusEndpoint, id),
                    new FormUrlEncodedContent(new Dictionary<string, string>()));
                _logger.LogInformation("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogError("Start not sent: " + e.Message);
            }
            return Ok();
        }
    }
}
