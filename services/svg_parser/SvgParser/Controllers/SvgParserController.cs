using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Svg;
using SvgParser.Models;
using SvgParser.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SvgParser.Controllers
{

    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class SvgParserController : ControllerBase
    {
        private readonly ILogger<SvgParserController> _logger;
        private readonly FileService _fileService;
        private readonly ConfigService _configService;
        private readonly ImageGeneratorService _generatorService;
        private readonly PropertySettings _propertySettings;

        public SvgParserController(ILogger<SvgParserController> logger, 
            FileService fileService, ConfigService configService, 
            ImageGeneratorService generatorService, PropertySettings propertySettings)
        {
            _logger = logger;
            _fileService = fileService;
            _configService = configService;
            _generatorService = generatorService;
            _propertySettings = propertySettings;
        }

        [HttpPost("startSvg/{id}")]
        public async Task<IActionResult> PostStart(string id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            _logger.LogInformation("id = {}", id);
            await SendStatus(id, _propertySettings.StatusBeginMessage, accessToken);

            List<string> imageIds = new List<string>();
            string fileId = await _configService.GetFileId(id);
            if (fileId == null) return UnprocessableEntity();
            SvgDocument document;
            using (Stream fileStream = await _fileService.Download(fileId))
            {
                document = SvgDocument.Open<SvgDocument>(fileStream);
            }
            int maxPages = _generatorService.GetMaxPage(document);
            _logger.LogInformation("maxPages: {}", maxPages);
            for (int i = 1; i <= maxPages; i++)
            {
                _generatorService.SetVisibilityForPage(ref document, i);

                using var stream = new MemoryStream();
                document.Draw().Save(stream, ImageFormat.Png);
                string imgId = await _fileService.Upload(i.ToString() + ".png", stream.ToArray());
                imageIds.Add(imgId);
            }

            await SendStatus(id, _propertySettings.StatusEndMessage, accessToken);

            // Call endpoint
            var body = new Dictionary<string, List<string>>() {
                { _propertySettings.FinishedIdsName, imageIds }
            };

            try
            {

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(
                    string.Format(_propertySettings.FinishedEndpoint, id),
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
                _logger.LogInformation("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogError("Finish message not sent: {}", e.Message);
            }
            return Ok();
        }

        private async Task SendStatus(string id, string message, string accessToken)
        {
            try
            {
                var content = new Dictionary<string, string>() { 
                    { _propertySettings.StatusProp, message }
                };
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var result = await client.PostAsync(string.Format(_propertySettings.StatusEndpoint, id),
                    new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
                _logger.LogDebug("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Status not sent: {}", e.Message);
            }
        }
    }
}
