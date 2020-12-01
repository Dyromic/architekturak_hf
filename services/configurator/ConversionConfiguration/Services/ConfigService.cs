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
        private readonly IMongoCollection<FileEntity> _files;
        private readonly IMongoCollection<StatusEntity> _status;

        public ConfigService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _configs = database.GetCollection<ConfigEntity>(settings.ConfigsCollectionName);
            _files = database.GetCollection<FileEntity>(settings.FilesCollectionName);
            _status = database.GetCollection<StatusEntity>(settings.StatusCollectionName);
        }

        private FileDto FileEntityToFileDto(FileEntity f)
        {
            if (f == null) return null;
            return new FileDto()
            {
                Id = f._id,
                Name = f.Name
            };
        }

        private string StatusEntityToString(StatusEntity s)
        {
            if (s == null) return null;
            return s.Status;
        }

        public List<ConfigWithFilesDTO> Get(Guid userId)
        {

            var doc = from p in _configs.AsQueryable()
                      join f1 in _files.AsQueryable() on p.PptFileId equals f1.FileId into joinedPptFile
                      join f2 in _files.AsQueryable() on p.SvgFileId equals f2.FileId into joinedSvgFile
                      join s in _status.AsQueryable() on p._id equals s.ConfigId into joinedStatus
                      select new ConfigWithFilesDTO()
                      {
                          ID = p._id,
                          AfterSlide = p.AfterSlide,
                          Animation = p.Animation,
                          MaxImages = p.MaxImages,
                          PptFileId = p.PptFileId,
                          SvgFileId = p.SvgFileId,
                          PptFile = joinedPptFile,
                          SvgFile = joinedSvgFile,
                          Status = joinedStatus 

                      };

            //_configs.AsQueryable().Join()

            return doc.ToList();

            /*return await _configs.Find(c => c.UserId == userId).Project(e => new ConfigDto
            {
                AfterSlide = e.AfterSlide,
                Animation = e.Animation,
                MaxImages = e.MaxImages,
                PptFileId = e.PptFileId,
                SvgFileId = e.SvgFileId
            }).FirstOrDefaultAsync() ?? new ConfigDto();*/
        }

        public ConfigWithFilesDTO Get(Guid userId, string id) {

            var doc = from p in _configs.AsQueryable()
                      where p._id == id
                      join f1 in _files.AsQueryable() on p.PptFileId equals f1.FileId into joinedPptFile
                      join f2 in _files.AsQueryable() on p.SvgFileId equals f2.FileId into joinedSvgFile
                      join s in _status.AsQueryable() on p._id equals s.ConfigId into joinedStatus
                      select new ConfigWithFilesDTO()
                      {
                          ID = p._id,
                          AfterSlide = p.AfterSlide,
                          Animation = p.Animation,
                          MaxImages = p.MaxImages,
                          PptFileId = p.PptFileId,
                          SvgFileId = p.SvgFileId,
                          PptFile = joinedPptFile,
                          SvgFile = joinedSvgFile,
                          Status = joinedStatus

                      };
            return doc.SingleOrDefault();

        }

        public async Task<string> Put(ConfigDto config, Guid userId)
        {
            ConfigEntity entity = new ConfigEntity
            {
                AfterSlide = config.AfterSlide,
                Animation = config.Animation,
                MaxImages = config.MaxImages,
                PptFileId = config.PptFileId,
                SvgFileId = config.SvgFileId,
                UserId = userId
            };
            await _configs.InsertOneAsync(entity);
            return entity._id;
        }
    }
}
