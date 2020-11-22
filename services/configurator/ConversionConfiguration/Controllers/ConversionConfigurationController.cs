using api;
using ConversionConfiguration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Controllers
{
    [ApiController]
    public class ConversionConfigurationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ConversionConfigurationController> _logger;

        public ConversionConfigurationController(ILogger<ConversionConfigurationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("files")]
        public async Task<ActionResult<List<FileElement>>> GetFiles()
        {
            throw new NotImplementedException();
        }

        [HttpPost("files")]
        public async Task<IActionResult> PostFiles(IFormFile file)
        {
            throw new NotImplementedException();
        }

        [HttpGet("configuration")]
        public async Task<ActionResult<ConfigurationDto>> GetConfigutation()
        {
            throw new NotImplementedException();
        }

        [HttpPut("configuration")]
        public async Task<IActionResult> PostConfiguration(ConfigurationDto config)
        {
            throw new NotImplementedException();
        }
    }
}
