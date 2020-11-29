using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Models
{
    public class UserEntity
    {
        [BsonId]
        public Guid ID { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string FirstName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string LastName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string PasswordHash { get; set; }

        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordSalt { get; set; }

    }

}
