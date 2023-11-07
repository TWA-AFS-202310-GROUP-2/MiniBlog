using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;

namespace MiniBlogTest.ControllerTest
{
    [Collection("IntegrationTest")]
    public class UserControllerTest : TestBase
    {
        private readonly Mock<IArticleRepository> mockArticleRepository;
        private readonly Mock<IUserRepository> mockUserRepository;

        private readonly ArticleStore articleStore;
        private readonly UserStore userStore;
        public UserControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)

        {
            mockArticleRepository = new Mock<IArticleRepository>();
            mockUserRepository = new Mock<IUserRepository>();
            articleStore = new ArticleStore();
            userStore = new UserStore(new List<User>());
        }

        [Fact]
        public async Task Should_get_all_users()
        {
            // given
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));

            // when
            var response = await client.GetAsync("/user");

            // then
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(body);
            Assert.True(users.Count == 0);
        }

        [Fact]
        public async Task Should_register_user_success()
        {
            // given
            var userName = "Tom";
            var email = "a@b.com";
            var user = new User(userName, email);
            mockUserRepository.Setup(mu => mu.CreateUser(user)).Returns(Task.FromResult(user));
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()),
                mockArticleRepository.Object,mockUserRepository.Object);

            // when
            var registerResponse = await client.PostAsJsonAsync("/user", user);
            
            var newUser = await registerResponse.Content.ReadFromJsonAsync<User>();

            Assert.Equal(user.Name,newUser.Name);
        }

        [Fact]
        public async Task Should_register_user_fail_when_UserStore_unavailable()
        {
            var client = GetClient(new ArticleStore(), null);

            var userName = "Tom";
            var email = "a@b.com";
            var user = new User(userName, email);
            var userJson = JsonConvert.SerializeObject(user);

            StringContent content = new StringContent(userJson, Encoding.UTF8, MediaTypeNames.Application.Json);
            var registerResponse = await client.PostAsync("/user", content);
            Assert.Equal(HttpStatusCode.InternalServerError, registerResponse.StatusCode);
        }

        [Fact]
        public async Task Should_update_user_email_success_()
        {
            mockUserRepository.Setup(mu => mu.GetUsers()).ReturnsAsync(new List<User>{new User("Tom","tom@b.com")});
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()),mockArticleRepository.Object,mockUserRepository.Object);

            var userName = "Tom";
            var originalEmail = "a@b.com";
            var updatedEmail = "tom@b.com";
            var originalUser = new User(userName, originalEmail);

            var newUser = new User(userName, updatedEmail);
            var registerResponse = await client.PostAsJsonAsync("/user", originalUser);

            await client.PutAsJsonAsync("/user", newUser);

            var response = await client.GetAsync("/user");

            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            Assert.True(users.Count == 1);
            Assert.Equal(updatedEmail, users[0].Email);
            Assert.Equal(userName, users[0].Name);
        }

        [Fact]
        public async Task Should_delete_user_and_related_article_success()
        {
            var client = GetClient(articleStore, userStore, mockArticleRepository.Object, mockUserRepository.Object);
            // given
            var userName = "Tom";

            mockArticleRepository.Setup(ma => ma.GetArticles()).Returns(Task.FromResult(new List<Article>
            {
                new Article("1", "1", "1"),
                new Article("2", "2", "2"),
            }));

            var articlesResponse = await client.GetAsync("/article");

            articlesResponse.EnsureSuccessStatusCode();
            var articles = JsonConvert.DeserializeObject<List<Article>>(
                await articlesResponse.Content.ReadAsStringAsync());
            Assert.Equal(2, articles.Count);

            mockUserRepository.Setup(ma => ma.GetUsers()).Returns(Task.FromResult(new List<User>
            {
                new User("1", "1"),
                new User("1", "1"),
                new User("1", "1")

            }));

            var userResponse = await client.GetAsync("/user");
            userResponse.EnsureSuccessStatusCode();
            var users = JsonConvert.DeserializeObject<List<User>>(
                await userResponse.Content.ReadAsStringAsync());
            Assert.True(users.Count == 1);

            // when
            await client.DeleteAsync($"/user?name={userName}");

            // then
            var articlesResponseAfterDeletion = await client.GetAsync("/article");
            articlesResponseAfterDeletion.EnsureSuccessStatusCode();
            var articlesLeft = JsonConvert.DeserializeObject<List<Article>>(
                await articlesResponseAfterDeletion.Content.ReadAsStringAsync());
            Assert.True(articlesLeft.Count == 0);

            var userResponseAfterDeletion = await client.GetAsync("/user");
            userResponseAfterDeletion.EnsureSuccessStatusCode();
            var usersLeft = JsonConvert.DeserializeObject<List<User>>(
                await userResponseAfterDeletion.Content.ReadAsStringAsync());
            Assert.True(usersLeft.Count == 0);
        }
    }
}