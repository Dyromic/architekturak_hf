using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvgParser.Models
{
    public class PropertySettings
    {
        public string SvgFileIdPropName { get; set; }
        public string PagesAttributeName { get; set; }
        public string FinishedEndpoint { get; set; }
        public string FinishedIdsName { get; set; }
    }
}
