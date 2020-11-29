using Aspose.Slides;
using Aspose.Slides.Export;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PptExporter.Models;
using PptExporter.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PptExporter.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class PptExporterController : ControllerBase
    {
        FileService _fileService;
        ConfigService _configService;
        PropertySettings _propertySettings;
        ILogger<PptExporterController> _logger;

        public PptExporterController(ILogger<PptExporterController> logger,
            FileService fileService, ConfigService configService,
            PropertySettings propertySettings)
        {
            _fileService = fileService;
            _configService = configService;
            _propertySettings = propertySettings;
            _logger = logger;
        }

        [HttpPost("startPpt/{id}")]
        public async Task<IActionResult> PostStart(string id, [FromForm] string[] ids)
        {
            await SendStatus(id, _propertySettings.StatusBeginMessage);

            ConfigDto config = await _configService.GetConfig(id);
            IPresentation presentation = null;
            if (config.PptId != null)
            {
                using (Stream fileStream = await _fileService.Download(config.PptId))
                {
                    try
                    {
                        presentation = new PresentationFactory().ReadPresentation(fileStream);
                    }
                    // Ne álljon le, ha nincs beadva PPT
                    catch (Exception) { }
                }
            }
            if (presentation == null)
            {
                presentation = new PresentationFactory().CreatePresentation();
            }

            for (int i = 0; i < ids.Length; i++)
            {
                ISlide slide = presentation.Slides.InsertEmptySlide(config.AfterSlide + i, presentation.LayoutSlides[0]);
                using Stream imageStream = await _fileService.Download(ids[i]);
                IPPImage img = presentation.Images.AddImage(imageStream);
                var size = presentation.SlideSize.Size;
                slide.Shapes.AddPictureFrame(ShapeType.Rectangle, size.Width - img.Width / 2,
                    size.Height - img.Height / 2, img.Width, img.Height, img);
            }

            using MemoryStream endFileStream = new MemoryStream();
            presentation.Save(endFileStream, SaveFormat.Pptx);
            string endFilename = "Result_" + id + ".pptx";
            string endFileId = await _fileService.Upload(endFilename, endFileStream.ToArray());
            await _fileService.SaveEndId(endFileId, endFilename);

            await SendStatus(id, _propertySettings.StatusEndMessage);
            return Ok();
        }

        private async Task SendStatus(string id, string message)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(
                    new Dictionary<string, string> { { _propertySettings.StatusProp, message } }),
                    Encoding.UTF8, "application/json");
                var result = await new HttpClient().PostAsync(
                    string.Format(_propertySettings.StatusEndpoint, id), content);
                _logger.LogDebug("Response: {}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Status not sent: " + e.Message);
            }
        }
    }
}
