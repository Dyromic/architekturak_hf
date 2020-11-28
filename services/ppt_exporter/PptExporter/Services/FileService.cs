using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PptExporter.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PptExporter.Services
{
    public class FileService
    {
        private IGridFSBucket _bucket;
        private readonly IMongoCollection<dynamic> _endFiles;
        private PropertySettings _propertySettings;

        public FileService(DatabaseSettings settings, PropertySettings propertySettings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _endFiles = database.GetCollection<dynamic>(settings.EndCollectionName);
            _bucket = new GridFSBucket(database);
            _propertySettings = propertySettings;
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

        public async Task SaveEndId(string fileId, string fileName)
        {
            dynamic doc = new ExpandoObject();
            ((IDictionary<string, object>)doc).Add(_propertySettings.EndIdName, fileId);
            ((IDictionary<string, object>)doc).Add(_propertySettings.EndFileName, fileName);

            await _endFiles.InsertOneAsync(doc);
        }
    }
}
