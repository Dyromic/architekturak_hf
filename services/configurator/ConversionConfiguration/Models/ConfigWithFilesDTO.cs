using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Models
{

    public class ConfigWithFilesDTO
    {
        public string ID { get; set; }
        public int AfterSlide { get; set; }
        public int MaxImages { get; set; }
        public string PptFileId { get; set; }
        public string SvgFileId { get; set; }
        public string Animation { get; set; }
        public IEnumerable<FileEntity> SvgFile { get; set; }
        public IEnumerable<FileEntity> PptFile { get; set; }
        public IEnumerable<StatusEntity> Status { get; set; }

    }
}
