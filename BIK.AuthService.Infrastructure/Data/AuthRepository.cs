using BIK.AuthService.Application.Interfaces;
using BIK.AuthService.Domain.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Conventions;

namespace BIK.AuthService.Infrastructure.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<AuthUser> _usersCollection;

        public AuthRepository(string connectionString, string databaseName)
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            if (!BsonClassMap.IsClassMapRegistered(typeof(AuthUser)))
            {
                BsonClassMap.RegisterClassMap<AuthUser>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId));
                });
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _usersCollection = database.GetCollection<AuthUser>("users"); 
        }

        public async Task<AuthUser?> GetUserByIdentifierAsync(string identifier)
        {
            var filter = Builders<AuthUser>.Filter.Or(
                Builders<AuthUser>.Filter.Eq(u => u.Dpi, identifier),
                Builders<AuthUser>.Filter.Eq(u => u.Email, identifier),
                Builders<AuthUser>.Filter.Eq(u => u.Telefono, identifier)
            );

            return await _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateCredentialsAsync(AuthUser user)
        {
            await _usersCollection.InsertOneAsync(user);
        }
    }
}