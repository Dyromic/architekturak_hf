using Aspose.Slides;
using Aspose.Slides.Export;
using Microsoft.AspNetCore.Mvc;
using PptExporter.Models;
using PptExporter.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Controllers
{
    [ApiController]
    public class PptExporterController : ControllerBase
    {
        FileService _fileService;
        ConfigService _configService;
        PropertySettings _propertySettings;

        public PptExporterController(FileService fileService, ConfigService configService,
            PropertySettings propertySettings)
        {
            _fileService = fileService;
            _configService = configService;
            _propertySettings = propertySettings;
        }

        [HttpPost("startPpt/{id}")]
        public async Task<IActionResult> PostStart(string id, [FromBody] string[] ids)
        {
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
                    // Ne álljon le, ha bármilyen hiba van
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

            return Ok();
        }
    }
}
