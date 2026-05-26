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
    /// <summary>
    /// Repositorio de persistencia en MongoDB para la gestión de usuarios, credenciales y notificaciones de seguridad.
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<AuthUser> _usersCollection;
        private readonly IMongoCollection<BsonDocument> _notificationsCollection;

        /// <summary>
        /// Inicializa la conexión a la base de datos de MongoDB, configura las convenciones de CamelCase y el mapeo de la entidad AuthUser.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión al servidor de MongoDB.</param>
        /// <param name="databaseName">Nombre de la base de datos.</param>
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
            _notificationsCollection = database.GetCollection<BsonDocument>("notifications");
        }

        /// <summary>
        /// Recupera un usuario por su identificador único (ID de base de datos, DPI, correo electrónico o teléfono).
        /// </summary>
        /// <param name="identifier">Identificador único en formato string.</param>
        /// <returns>La entidad de autenticación del usuario si existe, de lo contrario null.</returns>
        public async Task<AuthUser?> GetUserByIdentifierAsync(string identifier)
        {
            FilterDefinition<AuthUser> filter;

            if (ObjectId.TryParse(identifier, out _))
            {
                filter = Builders<AuthUser>.Filter.Or(
                    Builders<AuthUser>.Filter.Eq(u => u.Id, identifier),
                    Builders<AuthUser>.Filter.Eq(u => u.Dpi, identifier),
                    Builders<AuthUser>.Filter.Eq(u => u.Email, identifier),
                    Builders<AuthUser>.Filter.Eq(u => u.Telefono, identifier)
                );
            }
            else
            {
                filter = Builders<AuthUser>.Filter.Or(
                    Builders<AuthUser>.Filter.Eq(u => u.Dpi, identifier),
                    Builders<AuthUser>.Filter.Eq(u => u.Email, identifier),
                    Builders<AuthUser>.Filter.Eq(u => u.Telefono, identifier)
                );
            }

            return await _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Registra e inicializa el hash de contraseña, rol y estado inicial de verificación para el usuario.
        /// </summary>
        /// <param name="user">Entidad de autenticación con credenciales a actualizar.</param>
        public async Task CreateCredentialsAsync(AuthUser user)
        {
            var filter = Builders<AuthUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<AuthUser>.Update
                .Set(u => u.PasswordHash, user.PasswordHash)
                .Set(u => u.Rol, user.Rol)
                .Set(u => u.Estado, user.Estado);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Genera y persiste una notificación transaccional de auditoría por inicio de sesión.
        /// </summary>
        /// <param name="userId">Identificador del usuario que inició sesión.</param>
        public async Task CreateLoginNotificationAsync(string userId)
        {
            var notification = new BsonDocument
            {
                { "usuarioId", new ObjectId(userId) },
                { "titulo", "Nuevo inicio de sesión" },
                { "mensaje", $"Se detectó un nuevo inicio de sesión el {System.DateTime.Now:dd/MM/yyyy HH:mm}." },
                { "tipo", "Seguridad" },
                { "leido", false },
                { "createdAt", System.DateTime.UtcNow },
                { "updatedAt", System.DateTime.UtcNow }
            };

            await _notificationsCollection.InsertOneAsync(notification);
        }

        /// <summary>
        /// Reemplaza la información del usuario en MongoDB.
        /// </summary>
        /// <param name="user">Entidad del usuario a actualizar.</param>
        public async Task UpdateUserAsync(AuthUser user)
        {
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }
    }
}