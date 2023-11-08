using MiniBlog.Model;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User> GetByName(string name);
    }
}