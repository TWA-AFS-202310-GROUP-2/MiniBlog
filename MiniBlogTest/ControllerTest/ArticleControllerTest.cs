using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
        public ArticleControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
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
            //given
            var newArticle = new Article("Jerry", "Let's code", "c#");
            var mockArticleRepository = new Mock<IArticleRepository>();
            mockArticleRepository.Setup(r => r.CreateArticle(newArticle)).Returns(Task.FromResult(newArticle));

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(r => r.GetByName(newArticle.UserName)).Returns(Task.FromResult<User>(null));
            var client = GetClient(new ArticleStore(), new UserStore(), mockArticleRepository.Object, mockUserRepository.Object);

            var createArticleResponse = await client.PostAsJsonAsync("/article", newArticle);

            // It fail, please help
            Assert.Equal(HttpStatusCode.Created, createArticleResponse.StatusCode);

            //var article = await createArticleResponse.Content.ReadFromJsonAsync<Article>();
            //Assert.Equal(newArticle.Title, article.Title);
            //Assert.Equal(newArticle.Content, article.Content);
            //Assert.Equal(newArticle.UserName, article.UserName);
        }
    }
}
