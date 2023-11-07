using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Stores;
using Newtonsoft.Json;
using Xunit;

namespace MiniBlogTest.ControllerTest
{
    [Collection("IntegrationTest")]
    public class UserControllerTest : TestBase
    {
        public UserControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)

        {
        }

        [Fact]
        public async Task Should_get_all_users()
        {
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));
            var response = await client.GetAsync("/user");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(body);
            Assert.Empty(users);
        }

        [Fact]
        public async Task Should_register_user_success()
        {
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));

            var userName = "Tom";
            var email = "a@b.com";
            var user = new User(userName, email);
            var userJson = JsonConvert.SerializeObject(user);

            StringContent content = new StringContent(userJson, Encoding.UTF8, MediaTypeNames.Application.Json);
            var registerResponse = await client.PostAsync("/user", content);

            // It fail, please help
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

            var users = await GetUsers(client);
            Assert.Single(users);
            Assert.Equal(email, users[0].Email);
            Assert.Equal(userName, users[0].Name);
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
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));

            var userName = "Tom";
            var originalEmail = "a@b.com";
            var updatedEmail = "tom@b.com";
            var originalUser = new User(userName, originalEmail);

            var newUser = new User(userName, updatedEmail);
            StringContent registerUserContent = new StringContent(JsonConvert.SerializeObject(originalUser), Encoding.UTF8, MediaTypeNames.Application.Json);
            var registerResponse = await client.PostAsync("/user", registerUserContent);

            StringContent updateUserContent = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PutAsync("/user", updateUserContent);

            var users = await GetUsers(client);
            Assert.Single(users);
            Assert.Equal(updatedEmail, users[0].Email);
            Assert.Equal(userName, users[0].Name);
        }

        [Fact]
        public async Task Should_delete_user_and_related_article_success()
        {
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));

            var userName = "Tom";

            await PrepareArticle(new Article(userName, string.Empty, string.Empty), client);
            await PrepareArticle(new Article(userName, string.Empty, string.Empty), client);

            var articles = await GetArticles(client);
            Assert.Equal(4, articles.Count);

            var users = await GetUsers(client);
            Assert.Single(users);

            await client.DeleteAsync($"/user?name={userName}");

            var articlesAfterDeleteUser = await GetArticles(client);
            Assert.Equal(2, articlesAfterDeleteUser.Count);

            var usersAfterDeleteUser = await GetUsers(client);
            Assert.Empty(usersAfterDeleteUser);
        }

        private static async Task<List<User>> GetUsers(HttpClient client)
        {
            var response = await client.GetAsync("/user");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(body);
            return users;
        }

        private static async Task<List<Article>> GetArticles(HttpClient client)
        {
            var articleResponse = await client.GetAsync("/article");
            var articlesJson = await articleResponse.Content.ReadAsStringAsync();
            var articles = JsonConvert.DeserializeObject<List<Article>>(articlesJson);
            return articles;
        }

        private static async Task PrepareArticle(Article article1, HttpClient client)
        {
            StringContent registerUserContent = new StringContent(JsonConvert.SerializeObject(article1), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/article", registerUserContent);
        }
    }
}
