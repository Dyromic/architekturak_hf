using Auth.DTO;
using Auth.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<UserEntity> _users;

        public AuthService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<UserEntity>(settings.UsersCollectionName);
        }

        public async Task<string> Login(UserLogin user)
        {
            
            var foundUser = await _users.Find(u => u.Email == user.Email).FirstAsync();
            if (foundUser != null) {

                string hash = CreatePasswordHash(user.Password, foundUser.PasswordSalt);
                if (hash.Equals(foundUser.PasswordHash)) {
                    return GenerateJWT(foundUser);
                }

            }

            return null;
        }

        public async Task<string> Register(UserRegistration user)
        {
            var foundUser = await _users.Find(u => u.Email == user.Email).FirstAsync();
            if (foundUser != null) {
                return null;
            }
            
            throw new NotImplementedException();
        }

        private String GenerateJWT(UserEntity user) 
        {
            return "";
        }

        private string CreatePasswordHash(string password, string salt)
        {

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt));
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            return Encoding.UTF8.GetString(hmac.ComputeHash(passwordInBytes));

        }
    }
}
