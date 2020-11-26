using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversionConfiguration.Models
{
    public class ConfigEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int AfterSlide { get; set; }
        public int MaxImages { get; set; }
        public string PptFileId { get; set; }
        public string SvgFileId { get; set; }
        public string Animation { get; set; }
    }
}
