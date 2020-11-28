using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SvgParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SvgParser.Services
{
    public class ConfigService
    {
        private readonly IMongoCollection<dynamic> _configs;
        ILogger<ConfigService> _logger;
        PropertySettings _propSettings;

        public ConfigService(ILogger<ConfigService> logger, IDatabaseSettings dbSettings, PropertySettings propSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _configs = database.GetCollection<dynamic>(dbSettings.ConfigsCollectionName);
            _propSettings = propSettings;
            _logger = logger;
        }

        public async Task<string> GetFileId(string id)
        {
            IDictionary<string, object> result = null;
            try
            { 
                result = (IDictionary<string, object>)
                    await _configs.Find(Builders<dynamic>.Filter.Eq("_id", new ObjectId(id)))
                    .FirstOrDefaultAsync();
            }
            catch (InvalidCastException e)
            {
                _logger.LogError("Invalid cast: \n" + e);
                return null;
            }
            if (result?.ContainsKey(_propSettings.SvgFileIdPropName) ?? false)
            {
                return (string)result[_propSettings.SvgFileIdPropName];
            }
            return null;
        }
    }
}
