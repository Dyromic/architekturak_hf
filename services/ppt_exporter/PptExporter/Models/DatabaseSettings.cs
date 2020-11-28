using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Models
{
    public class DatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string EndCollectionName { get; set; }
        public string ConfigsCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
