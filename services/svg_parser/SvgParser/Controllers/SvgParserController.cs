using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Svg;
using SvgParser.Models;
using SvgParser.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SvgParser.Controllers
{
    [ApiController]
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
            await SendStatus(id, _propertySettings.StatusBeginMessage);

            List<string> imageIds = new List<string>();
            string fileId = await _configService.GetFileId(id);
            if (fileId == null) return UnprocessableEntity();
            SvgDocument document;
            using (Stream fileStream = await _fileService.Download(fileId))
            {
                document = SvgDocument.Open<SvgDocument>(fileStream);
            }
            int maxPages = _generatorService.GetMaxPage(document);
            for (int i = 1; i <= maxPages; i++)
            {
                _generatorService.SetVisibilityForPage(ref document, i);

                using var stream = new MemoryStream();
                document.Draw().Save(stream, ImageFormat.Png);
                string imgId = await _fileService.Upload(i.ToString() + ".png", stream.ToArray());
                imageIds.Add(imgId);
            }

            // Call endpoint
            HttpClient client = new HttpClient();
            var body = new Dictionary<string, string>();
            body.Add(_propertySettings.FinishedIdsName, imageIds.ToString());
            await client.PostAsync(_propertySettings.FinishedEndpoint + "/" + id, new FormUrlEncodedContent(body));

            await SendStatus(id, _propertySettings.StatusEndMessage);
            return Ok();
        }

        private async Task SendStatus(string id, string message)
        {
            try
            {
                var formVariables = new Dictionary<string, string>();
                formVariables.Add(_propertySettings.StatusProp, message);
                await new HttpClient().PostAsync(string.Format(_propertySettings.StatusEndpoint, id),
                    new FormUrlEncodedContent(formVariables));
            }
            catch (Exception) { }
        }
    }
}
