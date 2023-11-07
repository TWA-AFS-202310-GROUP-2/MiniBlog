using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Stores;
using Newtonsoft.Json;
using Xunit;
using MiniBlog.Repositories;
using Moq;
using System;
using MongoDB.Driver;

namespace MiniBlogTest.ControllerTest
{
    [Collection("IntegrationTest")]
    public class ArticleControllerTest : TestBase
    {
        private readonly Mock<IArticleRepository> mockArticleRepository;
        private readonly Mock<IUserRepository> mockUserRepository;

        private readonly ArticleStore articleStore;
        private readonly UserStore userStore;

        public ArticleControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            mockArticleRepository = new Mock<IArticleRepository>();
            mockUserRepository = new Mock<IUserRepository>();
            articleStore = new ArticleStore();
            userStore = new UserStore(new List<User>());
        }

        [Fact]
        public async void Should_get_all_Article()
        {
            var client = GetClient(articleStore, userStore, mockArticleRepository.Object, mockUserRepository.Object);
            mockArticleRepository.Setup(repo => repo.GetAll())
                                 .ReturnsAsync(
                                    new List<Article>
                                    {
                                        new Article("Tom", "Good day", "What a good day today!"),
                                        new Article("Tom", "Good day", "What a good day today!"),
                                    });
            var response = await client.GetAsync("/article");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<Article>>(body);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async void Should_create_article_fail_when_ArticleStore_unavailable()
        {
            var client = GetClient(articleStore, userStore, mockArticleRepository.Object, mockUserRepository.Object);
            mockArticleRepository.Setup(repo => repo.CreateArticle(It.IsAny<Article>()))
                                 .Throws(new System.Exception());
            string userNameWhoWillAdd = "Tom";
            string articleContent = "What a good day today!";
            string articleTitle = "Good day";
            Article article = new Article(userNameWhoWillAdd, articleTitle, articleContent);

            var httpContent = JsonConvert.SerializeObject(article);
            StringContent content = new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("/article", content);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async void Should_create_article_and_register_user_correct()
        {
            var client = GetClient(articleStore, userStore, mockArticleRepository.Object, mockUserRepository.Object);
            mockArticleRepository.Setup(repo => repo.CreateArticle(It.IsAny<Article>()))
                                 .ReturnsAsync((Article a) =>
                                 {
                                     a.Id = Guid.NewGuid();
                                     userStore.Users.Add(new User(a.UserName));
                                     return a;
                                 });
            mockArticleRepository.Setup(repo => repo.GetAll())
                                 .ReturnsAsync(
                                    new List<Article>
                                    {
                                        new Article("Tom", "Good day", "What a good day today!"),
                                        new Article("Tom", "Good day", "What a good day today!"),
                                        new Article("Tom", "Good day", "What a good day today!"),
                                    });
            mockUserRepository.Setup(repo => repo.GetUserByName(It.IsAny<string>()))
                              .ReturnsAsync(new User("Tom"));
            string userNameWhoWillAdd = "Tom";
            string articleContent = "What a good day today!";
            string articleTitle = "Good day";
            Article article = new Article(userNameWhoWillAdd, articleTitle, articleContent);

            var httpContent = JsonConvert.SerializeObject(article);
            StringContent content = new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var createArticleResponse = await client.PostAsync("/article", content);

            // It fail, please help
            Assert.Equal(HttpStatusCode.Created, createArticleResponse.StatusCode);

            var articleResponse = await client.GetAsync("/article");
            var body = await articleResponse.Content.ReadAsStringAsync();
            var articles = JsonConvert.DeserializeObject<List<Article>>(body);
            Assert.Equal(3, articles.Count);
            Assert.Equal(articleTitle, articles[2].Title);
            Assert.Equal(articleContent, articles[2].Content);
            Assert.Equal(userNameWhoWillAdd, articles[2].UserName);

            var userResponse = await client.GetAsync("/user");
            var usersJson = await userResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(usersJson);

            Assert.Equal(1, users.Count);
            Assert.Equal(userNameWhoWillAdd, users[0].Name);
            Assert.Equal("anonymous@unknow.com", users[0].Email);
        }
    }
}
