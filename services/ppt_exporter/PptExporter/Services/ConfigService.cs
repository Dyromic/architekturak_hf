using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PptExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Services
{
    public class ConfigService
    {
        private readonly IMongoCollection<dynamic> _configs;
        PropertySettings _propSettings;
        ILogger<ConfigService> _logger;

        public ConfigService(ILogger<ConfigService> logger, DatabaseSettings dbSettings, 
            PropertySettings propSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _configs = database.GetCollection<dynamic>(dbSettings.ConfigsCollectionName);
            _propSettings = propSettings;
            _logger = logger;
        }

        public async Task<ConfigDto> GetConfig(string id)
        {
            IDictionary<string, object> result;
            ConfigDto config = new ConfigDto();
            try
            {
                result = (IDictionary<string, object>)
                    await _configs.Find(Builders<dynamic>.Filter.Eq("_id", new ObjectId(id)))
                    .FirstOrDefaultAsync();
            }
            catch (InvalidCastException e)
            {
                _logger.LogError("Invalid cast: \n" + e);
                return config;
            }
            if (result == null)
            {
                _logger.LogError("Configuration not found");
                return config;
            }

            foreach (var item in result)
            {
                if (item.Key == _propSettings.AfterSlideProp) { 
                    if (int.TryParse(item.Value.ToString(), out int x)) 
                        config.AfterSlide = x;
                }
                else if (item.Key == _propSettings.AnimationProp)
                    switch (item.Value)
                    {
                        case "simple":
                            config.Animation = AnimationType.Simple;
                            break;
                        default:
                            break;
                    }
                else
                    _logger.LogInformation("Unknown key-value pair:" + item.Key + " = " + item.Value);
            }
            return config;
        }
    }
}
