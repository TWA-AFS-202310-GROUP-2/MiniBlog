using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Model;
using MiniBlog.Services;
using MiniBlog.Stores;

namespace MiniBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ArticleStore articleStore = null!;
        private readonly UserStore userStore = null!;

        public UserController(ArticleStore articleStore, UserStore userStore)
        {
            this.articleStore = articleStore;
            this.userStore = userStore;
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!userStore.Users.Exists(_ => user.Name.ToLower() == _.Name.ToLower()))
            {
                userStore.Users.Add(user);
            }

            return CreatedAtAction(nameof(GetByName), new { name = user.Name }, GetByName(user.Name));
        }

        [HttpGet]
        public List<User> GetAll()
        {
            return userStore.Users;
        }

        [HttpPut]
        public User Update(User user)
        {
            var foundUser = userStore.Users.FirstOrDefault(_ => _.Name == user.Name);
            if (foundUser != null)
            {
                foundUser.Email = user.Email;
            }

            return foundUser;
        }

        [HttpDelete]
        public User Delete(string name)
        {
            var foundUser = userStore.Users.FirstOrDefault(_ => _.Name == name);
            if (foundUser != null)
            {
                userStore.Users.Remove(foundUser);
            }

            return foundUser;
        }

        [HttpGet("{name}")]
        public User GetByName(string name)
        {
            return userStore.Users.FirstOrDefault(_ => _.Name.ToLower() == name.ToLower());
        }
    }
}
