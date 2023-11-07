using MiniBlog.Model;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetOneUser(string userName);
    }
}
