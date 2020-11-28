using ConversionConfiguration.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Services
{
    public class ConfigService
    {
        private readonly IMongoCollection<ConfigEntity> _configs;

        public ConfigService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _configs = database.GetCollection<ConfigEntity>(settings.ConfigsCollectionName);
        }

        public async Task<ConfigDto> Get()
        {
            return await _configs.Find(_ => true).Project(e => new ConfigDto
            {
                AfterSlide = e.AfterSlide,
                Animation = e.Animation,
                MaxImages = e.MaxImages,
                PptFileId = e.PptFileId,
                SvgFileId = e.SvgFileId
            }).FirstOrDefaultAsync() ?? new ConfigDto();
        }
            
        public async Task<string> Put(ConfigDto config)
        {
            ConfigEntity entity = new ConfigEntity
            {
                AfterSlide = config.AfterSlide,
                Animation = config.Animation,
                MaxImages = config.MaxImages,
                PptFileId = config.PptFileId,
                SvgFileId = config.SvgFileId
            };
            await _configs.InsertOneAsync(entity);
            return entity._id;
        }
    }
}
