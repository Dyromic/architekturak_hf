using Auth.DTO;
using Auth.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {

        private readonly IMongoCollection<UserEntity> _users;
        private readonly IConfiguration configuration;

        public AuthService(IDatabaseSettings settings, IConfiguration configuration)
        {
            this.configuration = configuration;
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
            if (!VerifyEmail(user.Email) || !VerifyPassword(user.Password)) return null;
            var foundUser = await _users.Find(u => u.Email == user.Email).FirstAsync();
            if (foundUser != null) {
                return null;
            }

            PasswordHash passwordHash = CreatePasswordHash(user.Password);

            UserEntity newUser = new UserEntity
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = passwordHash.Password,
                PasswordSalt = passwordHash.Salt
            };

            await _users.InsertOneAsync(newUser);

            return GenerateJWT(newUser);

        }

        public bool VerifyEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool VerifyPassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a - z])(?=.*[A - Z])(?=.*\d)(?=.*[^\da - zA - Z]).{ 8, 25 }$");
            return regex.Match(password).Success;
        }

        private String GenerateJWT(UserEntity user)
        {

            var mySecret = configuration["SecretKey"];
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "";
            var myAudience = "";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Surname, user.FirstName),
                    new Claim(ClaimTypes.GivenName, user.LastName)
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        
        }

        private string CreatePasswordHash(string password, string salt)
        {

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt));
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            return Encoding.UTF8.GetString(hmac.ComputeHash(passwordInBytes));

        }
        private PasswordHash CreatePasswordHash(string password)
        {

            using var hmac = new HMACSHA512();
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            return new PasswordHash {
                Password = Encoding.UTF8.GetString(hmac.ComputeHash(passwordInBytes)),
                Salt = Encoding.UTF8.GetString(hmac.Key)
            };

        }
    }
}
