using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
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

namespace MiniBlogTest.ControllerTest
{
    [Collection("IntegrationTest")]
    public class ArticleControllerTest : TestBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly Mock<IArticleRepository> _articleRepositoryMock;
        private readonly IUserRepository _userRepository;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        public ArticleControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async void Should_get_all_Article()
        {
            var mock = new Mock<IArticleRepository>();
            mock.Setup(repository => repository.GetArticles()).Returns(Task.FromResult(new List<Article>
            {
                new Article(null, "Happy new year", "Happy 2021 new year"),
                new Article(null, "Happy Halloween", "Halloween is coming"),
            }));
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), mock.Object);
            var response = await client.GetAsync("/article");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<Article>>(body);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async void Should_create_article_fail_when_ArticleStore_unavailable()
        {
            var client = GetClient(null, new UserStore(new List<User>()));
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
            _userRepositoryMock.Setup(r => r.GetUsers()).Returns(Task.FromResult(new List<User> { new User("Tom") }));

            var client = GetClient(new ArticleStore(new List<Article>
            {
                new Article(null, "Happy new year", "Happy 2021 new year"),
                new Article(null, "Happy Halloween", "Halloween is coming"),
            }), new UserStore(new List<User>()), _articleRepositoryMock.Object, _userRepositoryMock.Object);

            string userNameWhoWillAdd = "Tom";
            string articleContent = "What a good day today!";
            string articleTitle = "Good day";
            Article article = new Article(userNameWhoWillAdd, articleTitle, articleContent);

            var httpContent = JsonConvert.SerializeObject(article);
            StringContent content = new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            _articleRepositoryMock.Setup(r => r.GetArticles()).Returns(Task.FromResult(new List<Article>
            {
                new Article(null, "Happy new year", "Happy 2021 new year"),
                new Article(null, "Happy Halloween", "Halloween is coming"),
                new Article("Tom", "Good day", "What a good day today!"),
            }));

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

            _userRepositoryMock.Setup(ur => ur.GetUsers()).ReturnsAsync(new List<User>{new User("Tom")});

            var userResponse = await client.GetAsync("/user");
            var usersJson = await userResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(usersJson);


            Assert.Equal(1, users.Count);
            Assert.Equal(userNameWhoWillAdd, users[0].Name);
            Assert.Equal("anonymous@unknow.com", users[0].Email);
        }
    }
}
