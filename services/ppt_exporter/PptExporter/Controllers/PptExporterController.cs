using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PptExporter.Models;
using PptExporter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PptExporter.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class PptExporterController : ControllerBase
    {
        FileService _fileService;
        ConfigService _configService;
        PptService _pptService;
        PropertySettings _propertySettings;
        ILogger<PptExporterController> _logger;

        public PptExporterController(ILogger<PptExporterController> logger,
            FileService fileService, ConfigService configService,
            PropertySettings propertySettings, PptService pptService)
        {
            _fileService = fileService;
            _configService = configService;
            _pptService = pptService;
            _propertySettings = propertySettings;
            _logger = logger;
        }

        [HttpPost("startPpt/{id}")]
        public async Task<IActionResult> PostStart(string id, [FromBody] IdDto idsDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            string[] ids = idsDto.ids;
            _logger.LogInformation("id = {}, ids = {}", new object[] { id, ids });
            await SendStatus(id, _propertySettings.StatusBeginMessage, accessToken);

            ConfigDto config = await _configService.GetConfig(id);

            MemoryStream endFileStream = null;
            if (config.PptId != null)
            {
                using (Stream fileStream = await _fileService.Download(config.PptId))
                {
                    endFileStream = await _pptService.CreatePpt(fileStream, config, ids);
                }
            }
            if (endFileStream == null)
            {
                endFileStream = await _pptService.CreatePpt(null, config, ids);
            }
            _logger.LogDebug("stream: {}", endFileStream.ToArray());

            string endFilename = "Result_" + id + ".pptx";
            string endFileId = await _fileService.Upload(endFilename, endFileStream.ToArray());
            await _fileService.SaveEndId(endFileId, endFilename);

            await SendStatus(id, _propertySettings.StatusEndMessage, accessToken, endFileId);
            return Ok();
        }

        private async Task SendStatus(string id, string message, string accessToken)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(
                    new Dictionary<string, string> { { _propertySettings.StatusProp, message } }),
                    Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(
                    string.Format(_propertySettings.StatusEndpoint, id), content);
                _logger.LogDebug("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Status not sent: " + e.Message);
            }
        }

        private async Task SendStatus(string id, string message, string accessToken, string resultFileId)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(
                    new Dictionary<string, string> {
                        { _propertySettings.StatusProp, message },
                        { _propertySettings.ResultFileIdProp, resultFileId }
                    }
                ),
                    Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(
                    string.Format(_propertySettings.StatusDoneEndpoint, id), content);
                _logger.LogDebug("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Status not sent: " + e.Message);
            }
        }

        public class IdDto
        {
            public string[] ids { get; set; }
        }
    }
}
