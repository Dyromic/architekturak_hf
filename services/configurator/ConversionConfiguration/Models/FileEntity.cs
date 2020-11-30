using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConversionConfiguration.Models
{
    public class FileEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; }

        //[BsonId]
        public Guid UserId { get; set; }

        public string Name { get; set; }
    }
}
