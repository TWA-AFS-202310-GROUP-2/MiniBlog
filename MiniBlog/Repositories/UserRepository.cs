using System.Collections.Generic;
using System.Threading.Tasks;
using MiniBlog.Model;
using MongoDB.Driver;

namespace MiniBlog.Repositories;

public class UserRepository:IUserRepository
{
    private readonly IMongoCollection<User> userCollection = null!;

    public UserRepository(IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase("MiniBlog");

        userCollection = mongoDatabase.GetCollection<User>(User.CollectionName);
    }

    public async Task<List<User>> GetUsers()=>
      await userCollection.Find(_=>true).ToListAsync();

    public async Task<User?> GetUser(string userName)
    {
        return await userCollection.Find(u=>u.Name==userName).FirstOrDefaultAsync();
    }

    public async Task<User> CreateUser(User user)
    {
        await userCollection.InsertOneAsync(user);
        return await userCollection.Find(u=>u.Name ==user.Name).FirstAsync();
    }
}