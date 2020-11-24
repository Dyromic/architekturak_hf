﻿using ConversionConfiguration.Models;
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

        public async Task<List<FileDto>> Get()
        {
            return await _files.Find(_ => true)
                .Project(e => new FileDto { Id = e.FileId, Name = e.Name } )
                .ToListAsync();
        }
            
        public async Task<ObjectId> Add(string name, Stream stream)
        {
            ObjectId id = await _bucket.UploadFromStreamAsync(name, stream);
            await _files.InsertOneAsync(new FileEntity
            {
                FileId = id.ToString(),
                Name = name
            });
            return id;
        }
    }
}
