using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }

    public interface IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
    }

}
