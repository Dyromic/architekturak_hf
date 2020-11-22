using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Models
{
    public class ConfigurationDto
    {
        int AfterSlide { get; set; }
        int MaxImages { get; set; }
        int PptFileId { get; set; }
        int SvgFileId { get; set; }
        string Animation { get; set; }
    }
}
