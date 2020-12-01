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
    public class FileService
    {
        private readonly IMongoCollection<FileEntity> _files;
        private IGridFSBucket _bucket;

        public FileService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _files = database.GetCollection<FileEntity>(settings.FilesCollectionName);
            _bucket = new GridFSBucket(database);
        }

        public async Task<List<FileDto>> Get(Guid userID)
        {
            return await _files.Find(f => f.UserId == userID)
                .Project(e => new FileDto { Id = e.FileId, Name = e.Name } )
                .ToListAsync();
        }

        public async Task<Stream> GetStream(string id)
        {
            return await _bucket.OpenDownloadStreamAsync(new ObjectId(id));
        }

        public async Task<string> GetName(string id)
        {
            return await _files.Find(file => file.FileId == id)
                .Project(e => e.Name).SingleOrDefaultAsync();
        }

        public async Task<ObjectId> Add(string name, Guid userID, Stream stream)
        {
            ObjectId id = await _bucket.UploadFromStreamAsync(name, stream);
            await _files.InsertOneAsync(new FileEntity
            {
                FileId = id.ToString(),
                Name = name,
                UserId = userID
            });
            return id;
        }
        public async Task DebugAdd(string name, Guid userID)
        {
            await _files.InsertOneAsync(new FileEntity
            {
                FileId = "00000000000000000000000a",
                Name = name,
                UserId = userID
            });
        }
    }
}
