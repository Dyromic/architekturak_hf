using MongoDB.Bson;
using MongoDB.Driver;
using Status.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Status.Services
{
    public class StatusService
    {
        private readonly IMongoCollection<StatusEntity> _status;

        public StatusService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _status = database.GetCollection<StatusEntity>(settings.StatusCollectionName);
        }

        public async Task<string> Get(string id)
        {
            var res = await _status.Find(e => e.ConfigId.Equals(new ObjectId(id))).SingleOrDefaultAsync();
            return res?.Status ?? "";
        }

        public async Task Put(string id, string message)
        {
            var filter = new FilterDefinitionBuilder<StatusEntity>().Eq(s => s.ConfigId, id);
            var update = Builders<StatusEntity>.Update.Set(s => s.Status, message);
            if (await _status.Find(filter).AnyAsync())
            {
                await _status.UpdateOneAsync(filter, update);
            }
            else
            {
                await _status.InsertOneAsync(new StatusEntity { ConfigId = id, Status = message });
            }
        }
    }
}
