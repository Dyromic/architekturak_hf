using SvgParser.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;

namespace SvgParser.Services
{
    public class FileService
    {
        private IGridFSBucket _bucket;

        public FileService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _bucket = new GridFSBucket(database);
        }

        public async Task<Stream> Download(string fileId)
        {
            return await _bucket.OpenDownloadStreamAsync(new ObjectId(fileId));
        }
            
        public async Task<string> Upload(string name, byte[] bytes)
        {
            ObjectId id = await _bucket.UploadFromBytesAsync(name, bytes);
            return id.ToString();
        }
    }
}
