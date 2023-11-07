using System.Collections.Generic;
using System.Threading.Tasks;
using MiniBlog.Model;

namespace MiniBlog.Repositories;

public interface IUserRepository
{
    public Task<List<User>> GetUsers();

    public Task<User?> GetUser(string userName);

    public Task<User> CreateUser(User user);
}