using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string FilesCollectionName { get; set; }
        public string ConfigsCollectionName { get; set; }
        public string StatusCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }

    public interface IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string FilesCollectionName { get; set; }
        public string ConfigsCollectionName { get; set; }
        public string StatusCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
