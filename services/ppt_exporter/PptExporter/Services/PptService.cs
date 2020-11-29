using PptExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Packaging;
using Drawing = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml;
using Microsoft.Extensions.Logging;
using P = DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;
using NonVisualDrawingProperties = DocumentFormat.OpenXml.Presentation.NonVisualDrawingProperties;

namespace PptExporter.Services
{
    public class PptService
    {
        ILogger<PptService> _logger;
        FileService _fileService;

        public PptService(ILogger<PptService> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<MemoryStream> CreatePpt(Stream stream, ConfigDto config, string[] ids)
        {
            MemoryStream endFileStream = new MemoryStream();
            PresentationDocument presentation = null;
            if (stream != null)
            {
                try
                {
                    await stream.CopyToAsync(endFileStream);
                    presentation = PresentationDocument.Open(endFileStream, true);
                }
                catch (Exception)
                {
                    presentation = null;
                    _logger.LogWarning("Invalid file: {}", config.PptId);
                }
            }
            if (presentation == null)
            {
                presentation = PresentationDocument.Create(endFileStream, PresentationDocumentType.Presentation);
                PresentationPart presentationPart = presentation.AddPresentationPart();
                presentationPart.Presentation = new Presentation();
                CreatePresentationParts(presentationPart);
            }

            int maxSlides = config.MaxSlides > 0 ? Math.Min(ids.Length, config.MaxSlides) : ids.Length;
            for (int i = 0; i < maxSlides; i++)
            {
                using Stream imageStream = await _fileService.Download(ids[i]);
                InsertNewSlide(presentation, config.AfterSlide + i, imageStream);
            }
            presentation.Save();
            presentation.Close();
            //FileStream fs = new FileStream(filename, FileMode.Open);
            //fs.CopyTo(endFileStream);
            _logger.LogDebug("Stream in service: {}", endFileStream.ToArray());
            return endFileStream;
        }

        // Insert the specified slide into the presentation at the specified position.
        public static void InsertNewSlide(PresentationDocument presentationDocument, int position, Stream imageStream)
        {
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Verify that the presentation is not empty.
            if (presentationPart == null)
            {
                throw new InvalidOperationException("The presentation document is empty.");
            }

            // Declare and instantiate a new slide.
            Slide slide = new Slide(new CommonSlideData(new ShapeTree()));

            // Construct the slide content.            
            // Specify the non-visual properties of the new slide.
            NonVisualGroupShapeProperties nonVisualProperties = slide.CommonSlideData.ShapeTree.AppendChild(new NonVisualGroupShapeProperties());
            nonVisualProperties.NonVisualDrawingProperties = new NonVisualDrawingProperties() { Id = 1, Name = "" };
            nonVisualProperties.NonVisualGroupShapeDrawingProperties = new NonVisualGroupShapeDrawingProperties();
            nonVisualProperties.ApplicationNonVisualDrawingProperties = new ApplicationNonVisualDrawingProperties();

            // Specify the group shape properties of the new slide.
            slide.CommonSlideData.ShapeTree.AppendChild(new GroupShapeProperties());

            var slidePart = CreateSlidePart(presentationPart, imageStream);
            //var picture = CreatePicture(slidePart, imageStream);
            //slidePart.Slide.CommonSlideData.ShapeTree.Append(picture);

            // Save the new slide part.
            slide.Save(slidePart);

            // Modify the slide ID list in the presentation part.
            // The slide ID list should not be null.
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList ?? new SlideIdList();
            

            // Find the highest slide ID in the current list.
            uint maxSlideId = 1;
            SlideId prevSlideId = null;

            foreach (SlideId slideId in slideIdList.ChildElements)
            {
                if (slideId.Id > maxSlideId)
                {
                    maxSlideId = slideId.Id;
                }

                position--;
                if (position == 0)
                {
                    prevSlideId = slideId;
                }

            }

            maxSlideId++;

            // Get the ID of the previous slide.
            SlidePart lastSlidePart = null;

            if (prevSlideId != null)
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(prevSlideId.RelationshipId);
            }
            else if (slideIdList.ChildElements.Count != 0)
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(((SlideId)(slideIdList.ChildElements[0])).RelationshipId);
            }

            // Use the same slide layout as that of the previous slide.
            if (null != lastSlidePart && null != lastSlidePart.SlideLayoutPart)
            {
                slidePart.AddPart(lastSlidePart.SlideLayoutPart);
            }

            // Insert the new slide into the slide list after the previous slide.
            SlideId newSlideId = slideIdList.InsertAfter(new SlideId(), prevSlideId);
            newSlideId.Id = maxSlideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            // Save the modified presentation.
            presentationPart.Presentation.Save();
        }

        private static void CreatePresentationParts(PresentationPart presentationPart)
        {
            SlideMasterIdList slideMasterIdList1 = new SlideMasterIdList(new SlideMasterId() { Id = (UInt32Value)2147483648U, RelationshipId = "rId1" });
            SlideIdList slideIdList1 = new SlideIdList(new SlideId() { Id = (UInt32Value)256U, RelationshipId = "rId2" });
            SlideSize slideSize1 = new SlideSize() { Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen4x3 };
            NotesSize notesSize1 = new NotesSize() { Cx = 6858000, Cy = 9144000 };
            DefaultTextStyle defaultTextStyle1 = new DefaultTextStyle();

            presentationPart.Presentation.Append(slideMasterIdList1, slideIdList1, slideSize1, notesSize1, defaultTextStyle1);

            SlidePart slidePart1;
            SlideLayoutPart slideLayoutPart1;
            SlideMasterPart slideMasterPart1;
            ThemePart themePart1;


            slidePart1 = CreateSlidePart(presentationPart);
            slideLayoutPart1 = CreateSlideLayoutPart(slidePart1);
            slideMasterPart1 = CreateSlideMasterPart(slideLayoutPart1);
            themePart1 = CreateTheme(slideMasterPart1);

            slideMasterPart1.AddPart(slideLayoutPart1, "rId1");
            presentationPart.AddPart(slideMasterPart1, "rId1");
            presentationPart.AddPart(themePart1, "rId5");
        }

        private static SlidePart CreateSlidePart(PresentationPart presentationPart)
        {
            SlidePart slidePart1 = presentationPart.AddNewPart<SlidePart>("rId2");
            slidePart1.Slide = new Slide(
                    new CommonSlideData(
                        new ShapeTree(
                            new P.NonVisualGroupShapeProperties(
                                new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                                new P.NonVisualGroupShapeDrawingProperties(),
                                new ApplicationNonVisualDrawingProperties()),
                            new GroupShapeProperties(new Drawing.TransformGroup()),
                            new P.Shape(
                                new P.NonVisualShapeProperties(
                                    new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title 1" },
                                    new P.NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                                    new ApplicationNonVisualDrawingProperties(new PlaceholderShape())),
                                new P.ShapeProperties(),
                                new P.TextBody(
                                    new Drawing.BodyProperties(),
                                    new Drawing.ListStyle(),
                                    new Drawing.Paragraph(new Drawing.EndParagraphRunProperties() { Language = "en-US" }))))),
                    new ColorMapOverride(new Drawing.MasterColorMapping()));
            return slidePart1;
        }

        private static SlideLayoutPart CreateSlideLayoutPart(SlidePart slidePart1)
        {
            SlideLayoutPart slideLayoutPart1 = slidePart1.AddNewPart<SlideLayoutPart>("rId1");
            SlideLayout slideLayout = new SlideLayout(
            new CommonSlideData(new ShapeTree(
              new P.NonVisualGroupShapeProperties(
              new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
              new P.NonVisualGroupShapeDrawingProperties(),
              new ApplicationNonVisualDrawingProperties()),
              new GroupShapeProperties(new Drawing.TransformGroup()),
              new P.Shape(
              new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "" },
                new P.NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                new ApplicationNonVisualDrawingProperties(new PlaceholderShape())),
              new P.ShapeProperties(),
              new P.TextBody(
                new Drawing.BodyProperties(),
                new Drawing.ListStyle(),
                new Drawing.Paragraph(new Drawing.EndParagraphRunProperties()))))),
            new ColorMapOverride(new Drawing.MasterColorMapping()));
            slideLayoutPart1.SlideLayout = slideLayout;
            return slideLayoutPart1;
        }

        private static SlidePart CreateSlidePart(PresentationPart presentationPart, Stream imageStream)
        {
            SlidePart slidePart1 = presentationPart.AddNewPart<SlidePart>("rId3");
            slidePart1.Slide = new Slide(
             new CommonSlideData(
                 new ShapeTree(
                     new P.NonVisualGroupShapeProperties(
                         new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                         new P.NonVisualGroupShapeDrawingProperties(),
                         new ApplicationNonVisualDrawingProperties()),
                     new GroupShapeProperties(new Drawing.TransformGroup()),
                     new P.Shape(
                         new P.NonVisualShapeProperties(
                             new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title 1" },
                             new P.NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                             new ApplicationNonVisualDrawingProperties(new PlaceholderShape())),
                         new P.ShapeProperties(),
                         new P.TextBody(
                             new Drawing.BodyProperties(),
                             new Drawing.ListStyle(),
                             new Drawing.Paragraph(new Drawing.EndParagraphRunProperties() { Language = "en-US" }))))),
             new ColorMapOverride(new Drawing.MasterColorMapping()));
                        
            ImagePart imagePart = slidePart1.AddNewPart<ImagePart>("image/png", "rId3");
            imagePart.FeedData(imageStream);

            P.Picture picture = new P.Picture();
            P.BlipFill blipFill = new P.BlipFill();
            D.Blip blip = new D.Blip() { Embed = "rId3" };
            Drawing.Stretch stretch = new Drawing.Stretch();
            Drawing.FillRectangle fillRectangle = new Drawing.FillRectangle();
            stretch.Append(fillRectangle);
            blipFill.Append(blip);
            blipFill.Append(stretch);
            P.ShapeProperties shapeProperties1 = new P.ShapeProperties();

            D.Transform2D transform2D1 = new D.Transform2D();

            SlideSize size = presentationPart.Presentation.SlideSize;

            D.Offset offset1 = new D.Offset() { X = 200000L, Y = 200000L };
            D.Extents extents1 = new D.Extents() { Cx = size.Cx - 400000L, Cy = size.Cy - 400000L };

            transform2D1.Append(offset1);
            transform2D1.Append(extents1);

            D.PresetGeometry presetGeometry1 = new D.PresetGeometry() { Preset = D.ShapeTypeValues.Rectangle };
            D.AdjustValueList adjustValueList1 = new D.AdjustValueList();

            presetGeometry1.Append(adjustValueList1);

            shapeProperties1.Append(transform2D1);
            shapeProperties1.Append(presetGeometry1);

            NonVisualPictureProperties nonVisualPictureProperties1 = new NonVisualPictureProperties();
            NonVisualDrawingProperties nonVisualDrawingProperties9 = new NonVisualDrawingProperties() { Id = (UInt32Value)3U, Name = "Kép 2" };
            NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties1 = new NonVisualPictureDrawingProperties();
            ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties9 = new ApplicationNonVisualDrawingProperties();

            nonVisualPictureProperties1.Append(nonVisualDrawingProperties9);
            nonVisualPictureProperties1.Append(nonVisualPictureDrawingProperties1);
            nonVisualPictureProperties1.Append(applicationNonVisualDrawingProperties9);

            picture.Append(nonVisualPictureProperties1);
            picture.Append(blipFill);
            picture.Append(shapeProperties1);
            slidePart1.Slide.CommonSlideData.ShapeTree.Append(picture);
            
            return slidePart1;
        }

        private static SlideMasterPart CreateSlideMasterPart(SlideLayoutPart slideLayoutPart1)
        {
            SlideMasterPart slideMasterPart1 = slideLayoutPart1.AddNewPart<SlideMasterPart>("rId1");
            SlideMaster slideMaster = new SlideMaster(
            new CommonSlideData(new ShapeTree(
              new P.NonVisualGroupShapeProperties(
              new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
              new P.NonVisualGroupShapeDrawingProperties(),
              new ApplicationNonVisualDrawingProperties()),
              new GroupShapeProperties(new Drawing.TransformGroup()),
              new P.Shape(
              new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)2U, Name = "Title Placeholder 1" },
                new P.NonVisualShapeDrawingProperties(new Drawing.ShapeLocks() { NoGrouping = true }),
                new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title })),
              new P.ShapeProperties(),
              new P.TextBody(
                new Drawing.BodyProperties(),
                new Drawing.ListStyle(),
                new Drawing.Paragraph())))),
            new P.ColorMap() { Background1 = D.ColorSchemeIndexValues.Light1, Text1 = D.ColorSchemeIndexValues.Dark1, Background2 = D.ColorSchemeIndexValues.Light2, Text2 = D.ColorSchemeIndexValues.Dark2, Accent1 = D.ColorSchemeIndexValues.Accent1, Accent2 = D.ColorSchemeIndexValues.Accent2, Accent3 = D.ColorSchemeIndexValues.Accent3, Accent4 = D.ColorSchemeIndexValues.Accent4, Accent5 = D.ColorSchemeIndexValues.Accent5, Accent6 = D.ColorSchemeIndexValues.Accent6, Hyperlink = D.ColorSchemeIndexValues.Hyperlink, FollowedHyperlink = D.ColorSchemeIndexValues.FollowedHyperlink },
            new SlideLayoutIdList(new SlideLayoutId() { Id = (UInt32Value)2147483649U, RelationshipId = "rId1" }),
            new TextStyles(new TitleStyle(), new BodyStyle(), new OtherStyle()));
            slideMasterPart1.SlideMaster = slideMaster;

            return slideMasterPart1;
        }

        private static ThemePart CreateTheme(SlideMasterPart slideMasterPart1)
        {
            ThemePart themePart1 = slideMasterPart1.AddNewPart<ThemePart>("rId5");
            D.Theme theme1 = new D.Theme() { Name = "Office Theme" };

            D.ThemeElements themeElements1 = new D.ThemeElements(
            new D.ColorScheme(
              new D.Dark1Color(new D.SystemColor() { Val = D.SystemColorValues.WindowText, LastColor = "000000" }),
              new D.Light1Color(new D.SystemColor() { Val = D.SystemColorValues.Window, LastColor = "FFFFFF" }),
              new D.Dark2Color(new D.RgbColorModelHex() { Val = "1F497D" }),
              new D.Light2Color(new D.RgbColorModelHex() { Val = "EEECE1" }),
              new D.Accent1Color(new D.RgbColorModelHex() { Val = "4F81BD" }),
              new D.Accent2Color(new D.RgbColorModelHex() { Val = "C0504D" }),
              new D.Accent3Color(new D.RgbColorModelHex() { Val = "9BBB59" }),
              new D.Accent4Color(new D.RgbColorModelHex() { Val = "8064A2" }),
              new D.Accent5Color(new D.RgbColorModelHex() { Val = "4BACC6" }),
              new D.Accent6Color(new D.RgbColorModelHex() { Val = "F79646" }),
              new D.Hyperlink(new D.RgbColorModelHex() { Val = "0000FF" }),
              new D.FollowedHyperlinkColor(new D.RgbColorModelHex() { Val = "800080" }))
            { Name = "Office" },
              new D.FontScheme(
              new D.MajorFont(
              new D.LatinFont() { Typeface = "Calibri" },
              new D.EastAsianFont() { Typeface = "" },
              new D.ComplexScriptFont() { Typeface = "" }),
              new D.MinorFont(
              new D.LatinFont() { Typeface = "Calibri" },
              new D.EastAsianFont() { Typeface = "" },
              new D.ComplexScriptFont() { Typeface = "" }))
              { Name = "Office" },
              new D.FormatScheme(
              new D.FillStyleList(
              new D.SolidFill(new D.SchemeColor() { Val = D.SchemeColorValues.PhColor }),
              new D.GradientFill(
                new D.GradientStopList(
                new D.GradientStop(new D.SchemeColor(new D.Tint() { Val = 50000 },
                  new D.SaturationModulation() { Val = 300000 })
                { Val = D.SchemeColorValues.PhColor })
                { Position = 0 },
                new D.GradientStop(new D.SchemeColor(new D.Tint() { Val = 37000 },
                 new D.SaturationModulation() { Val = 300000 })
                { Val = D.SchemeColorValues.PhColor })
                { Position = 35000 },
                new D.GradientStop(new D.SchemeColor(new D.Tint() { Val = 15000 },
                 new D.SaturationModulation() { Val = 350000 })
                { Val = D.SchemeColorValues.PhColor })
                { Position = 100000 }
                ),
                new D.LinearGradientFill() { Angle = 16200000, Scaled = true }),
              new D.NoFill(),
              new D.PatternFill(),
              new D.GroupFill()),
              new D.LineStyleList(
              new D.Outline(
                new D.SolidFill(
                new D.SchemeColor(
                  new D.Shade() { Val = 95000 },
                  new D.SaturationModulation() { Val = 105000 })
                { Val = D.SchemeColorValues.PhColor }),
                new D.PresetDash() { Val = D.PresetLineDashValues.Solid })
              {
                  Width = 9525,
                  CapType = D.LineCapValues.Flat,
                  CompoundLineType = D.CompoundLineValues.Single,
                  Alignment = D.PenAlignmentValues.Center
              },
              new D.Outline(
                new D.SolidFill(
                new D.SchemeColor(
                  new D.Shade() { Val = 95000 },
                  new D.SaturationModulation() { Val = 105000 })
                { Val = D.SchemeColorValues.PhColor }),
                new D.PresetDash() { Val = D.PresetLineDashValues.Solid })
              {
                  Width = 9525,
                  CapType = D.LineCapValues.Flat,
                  CompoundLineType = D.CompoundLineValues.Single,
                  Alignment = D.PenAlignmentValues.Center
              },
              new D.Outline(
                new D.SolidFill(
                new D.SchemeColor(
                  new D.Shade() { Val = 95000 },
                  new D.SaturationModulation() { Val = 105000 })
                { Val = D.SchemeColorValues.PhColor }),
                new D.PresetDash() { Val = D.PresetLineDashValues.Solid })
              {
                  Width = 9525,
                  CapType = D.LineCapValues.Flat,
                  CompoundLineType = D.CompoundLineValues.Single,
                  Alignment = D.PenAlignmentValues.Center
              }),
              new D.EffectStyleList(
              new D.EffectStyle(
                new D.EffectList(
                new D.OuterShadow(
                  new D.RgbColorModelHex(
                  new D.Alpha() { Val = 38000 })
                  { Val = "000000" })
                { BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false })),
              new D.EffectStyle(
                new D.EffectList(
                new D.OuterShadow(
                  new D.RgbColorModelHex(
                  new D.Alpha() { Val = 38000 })
                  { Val = "000000" })
                { BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false })),
              new D.EffectStyle(
                new D.EffectList(
                new D.OuterShadow(
                  new D.RgbColorModelHex(
                  new D.Alpha() { Val = 38000 })
                  { Val = "000000" })
                { BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false }))),
              new D.BackgroundFillStyleList(
              new D.SolidFill(new D.SchemeColor() { Val = D.SchemeColorValues.PhColor }),
              new D.GradientFill(
                new D.GradientStopList(
                new D.GradientStop(
                  new D.SchemeColor(new D.Tint() { Val = 50000 },
                    new D.SaturationModulation() { Val = 300000 })
                  { Val = D.SchemeColorValues.PhColor })
                { Position = 0 },
                new D.GradientStop(
                  new D.SchemeColor(new D.Tint() { Val = 50000 },
                    new D.SaturationModulation() { Val = 300000 })
                  { Val = D.SchemeColorValues.PhColor })
                { Position = 0 },
                new D.GradientStop(
                  new D.SchemeColor(new D.Tint() { Val = 50000 },
                    new D.SaturationModulation() { Val = 300000 })
                  { Val = D.SchemeColorValues.PhColor })
                { Position = 0 }),
                new D.LinearGradientFill() { Angle = 16200000, Scaled = true }),
              new D.GradientFill(
                new D.GradientStopList(
                new D.GradientStop(
                  new D.SchemeColor(new D.Tint() { Val = 50000 },
                    new D.SaturationModulation() { Val = 300000 })
                  { Val = D.SchemeColorValues.PhColor })
                { Position = 0 },
                new D.GradientStop(
                  new D.SchemeColor(new D.Tint() { Val = 50000 },
                    new D.SaturationModulation() { Val = 300000 })
                  { Val = D.SchemeColorValues.PhColor })
                { Position = 0 }),
                new D.LinearGradientFill() { Angle = 16200000, Scaled = true })))
              { Name = "Office" });

            theme1.Append(themeElements1);
            theme1.Append(new D.ObjectDefaults());
            theme1.Append(new D.ExtraColorSchemeList());

            themePart1.Theme = theme1;
            return themePart1;

        }

        public static Picture CreatePicture(SlidePart slidePart, Stream stream)
        {
            ImagePart imagePart = slidePart.AddNewPart<ImagePart>("image/png", "rId2");
            imagePart.FeedData(stream);
            stream.Close();
            slidePart.Slide = new Slide(
                    new CommonSlideData(
                        new ShapeTree(
                            new P.NonVisualGroupShapeProperties(
                                new P.NonVisualDrawingProperties()
                                { Id = (UInt32Value)1U, Name = "" },
                                new P.NonVisualGroupShapeDrawingProperties(),
                                new ApplicationNonVisualDrawingProperties()),
                            new GroupShapeProperties(new Drawing.TransformGroup()),
                            new P.Shape(
                                new P.NonVisualShapeProperties(
                                    new P.NonVisualDrawingProperties()
                                    { Id = (UInt32Value)2U, Name = "Title 1" },
                                    new P.NonVisualShapeDrawingProperties(
                                        new Drawing.ShapeLocks() { NoGrouping = true }),
                                    new ApplicationNonVisualDrawingProperties(
                                        new PlaceholderShape())),
                                new P.ShapeProperties(),
                                new P.TextBody(
                                    new Drawing.BodyProperties(),
                                    new Drawing.ListStyle(),
                                    new Drawing.Paragraph(
                                        new Drawing.EndParagraphRunProperties()
                                        { Language = "en-US" }))))),
                    new ColorMapOverride(new Drawing.MasterColorMapping()));
            P.Picture picture = new P.Picture();
            P.BlipFill blipFill = new P.BlipFill();
            D.Blip blip = new D.Blip() { Embed = "rId2" };
            Drawing.Stretch stretch = new Drawing.Stretch();
            Drawing.FillRectangle fillRectangle = new Drawing.FillRectangle();
            stretch.Append(fillRectangle);
            blipFill.Append(blip);
            blipFill.Append(stretch);
            Drawing.BlipExtensionList blipExtensionList1 = new Drawing.BlipExtensionList();
            Drawing.BlipExtension blipExtension1 = new Drawing.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            var useLocalDpi1 = new DocumentFormat.OpenXml.Office2010.Drawing.UseLocalDpi() { Val = false };
            useLocalDpi1.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");

            blipExtension1.Append(useLocalDpi1);
            blipExtensionList1.Append(blipExtension1);
            blip.Append(blipExtensionList1);
            picture.Append(blipFill);
            P.ShapeProperties shapeProperties1 = new P.ShapeProperties();

            D.Transform2D transform2D1 = new D.Transform2D();
            D.Offset offset1 = new D.Offset() { X = 467544L, Y = 548680L };
            D.Extents extents1 = new D.Extents() { Cx = 3743848L, Cy = 3886743L };

            transform2D1.Append(offset1);
            transform2D1.Append(extents1);

            D.PresetGeometry presetGeometry1 = new D.PresetGeometry() { Preset = D.ShapeTypeValues.Rectangle };
            D.AdjustValueList adjustValueList1 = new D.AdjustValueList();

            presetGeometry1.Append(adjustValueList1);

            shapeProperties1.Append(transform2D1);
            shapeProperties1.Append(presetGeometry1);
            picture.Append(shapeProperties1);

            return picture;
        }
    }
}
