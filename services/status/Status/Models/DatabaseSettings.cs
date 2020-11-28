using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Status.Models
{
    public class DatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string StatusCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }
}
