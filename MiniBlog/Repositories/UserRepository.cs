using MiniBlog.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> userCollection;
        public UserRepository(IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase("MiniBlog");

            userCollection = mongoDatabase.GetCollection<User>(User.CollectionName);
        }

        public async Task<List<User>> GetUsers() =>
            await userCollection.Find(_ => true).ToListAsync();

        public async Task<User> GetOneUser(string name) =>
            await userCollection.Find(a => a.Name == name).FirstAsync();

        public async Task<User> CreateUser(User user)
        {
            await userCollection.InsertOneAsync(user);
            return await userCollection.Find(a => a.Name == user.Name).FirstAsync();
        }
    }
}
