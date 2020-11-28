using Svg;
using SvgParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvgParser.Services
{
    public class ImageGeneratorService
    {
        private PropertySettings _propertySettings;

        public ImageGeneratorService(PropertySettings propertySettings)
        {
            _propertySettings = propertySettings;
        }

        public void SetVisibilityForPage(ref SvgDocument document, int page)
        {
            document.ApplyRecursiveDepthFirst(element =>
            {
                if (element.CustomAttributes.TryGetValue(_propertySettings.PagesAttributeName, out string pages))
                {
                    if (ContainsPage(pages, page))
                    {
                        element.Visibility = "visible";
                    } 
                    else
                    {
                        element.Visibility = "hidden";
                    }
                }
            });
        }

        public int GetMaxPage(SvgDocument document)
        {
            int maxPage = 1;
            document.ApplyRecursiveDepthFirst(element =>
            {
                if (element.CustomAttributes.TryGetValue(_propertySettings.PagesAttributeName, out string pages))
                {
                    foreach (var row in pages.Split(","))
                    {
                        if (int.TryParse(row, out int page))
                        {
                            maxPage = page > maxPage ? page : maxPage;
                        }
                        else if (row.Contains(":") && int.TryParse(row.Split(":")[1], out int to))
                        {
                            maxPage = to > maxPage ? to : maxPage;
                        }
                    }
                }
            });
            return maxPage;
        }
        private bool ContainsPage(string pages, int page)
        {
            foreach (var element in pages.Split(","))
            {
                if (int.TryParse(element, out int ePage)) {
                    if (ePage == page) return true;
                }
                else
                {
                    var fromTo = element.Split(":");
                    int from = -1, to = -1;
                    if (int.TryParse(fromTo[0], out from) || int.TryParse(fromTo[1], out to))
                    {
                        if ( (from == -1 && page <= to) 
                            || (from <= page && to == -1)
                            || (from <= page && page <= to)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
