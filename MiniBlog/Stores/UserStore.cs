using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniBlog.Model;
using MiniBlog.Repositories;

namespace MiniBlog.Stores
{
    public class UserStore : IUserRepository
    {
        public UserStore()
        {
            Users = new List<User>
            {
                new User("Andrew", "1@1.com"),
                new User("William", "2@2.com"),
            };
        }

        public UserStore(List<User> users)
        {
            Users = users;
        }

        public List<User> Users { get; set; }

        public Task<User> Create(User user)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> GetByName(string name)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Name == name));
        }
    }
}
