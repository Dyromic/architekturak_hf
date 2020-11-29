using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Models
{
    public class PropertySettings
    {
        public string EndIdName { get; set; }
        public string EndFileName { get; set; }
        public string AfterSlideProp { get; set; }
        public string MaxSlidesProp { get; set; }
        public string AnimationProp { get; set; }
        public string StatusEndpoint { get; set; }
        public string StatusProp { get; set; }
        public string StatusBeginMessage { get; set; }
        public string StatusEndMessage { get; set; }
    }
}
