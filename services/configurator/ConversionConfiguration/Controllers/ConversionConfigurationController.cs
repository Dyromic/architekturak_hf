using ConversionConfiguration.Models;
using ConversionConfiguration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Controllers
{
    [ApiController]
    public class ConversionConfigurationController : ControllerBase
    {
        private readonly ILogger<ConversionConfigurationController> _logger;
        private readonly FileService _fileService;
        private readonly ConfigService _configService;

        public ConversionConfigurationController(ILogger<ConversionConfigurationController> logger,
            FileService fileService, ConfigService configService)
        {
            _logger = logger;
            _fileService = fileService;
            _configService = configService;
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
        public async Task<IActionResult> PutConfiguration(ConfigDto config)
        {
            await _configService.Put(config);
            return Ok();
        }
    }
}
