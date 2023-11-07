using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Services;
using MiniBlog.Stores;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace MiniBlogTest.ServiceTest;

public class ArticleServiceTest : TestBase
{
    public ArticleServiceTest(CustomWebApplicationFactory<Startup> factory)
        : base(factory)
    {
    }

    [Fact]
    public void Should_create_article_when_invoke_CreateArticle_given_input_article()
    {
        // given
        var newArticle = new Article("Jerry", "Let's code", "c#");
        var mock = new Mock<IArticleRepository>();
        mock.Setup(repository => repository.CreateArticle(newArticle)).Returns(Task.FromResult(newArticle));
        ArticleService articleService = new ArticleService(new ArticleStore(), new UserStore(new List<User>()), mock.Object);
        var articleStore = new ArticleStore();
        var articleCountBeforeAddNewOne = articleStore.Articles.Count;
        var userStore = new UserStore();
        //var articleService = new ArticleService(articleStore, userStore);

        // when
        var addedArticle = articleService.CreateArticle(newArticle);

        // then
        //Assert.Equal(articleCountBeforeAddNewOne + 1, articleStore.Articles.Count);
        Assert.Equal(newArticle.Title, addedArticle.Result.Title);
        Assert.Equal(newArticle.Content, addedArticle.Result.Content);
        Assert.Equal(newArticle.UserName, addedArticle.Result.UserName);
    }
}
