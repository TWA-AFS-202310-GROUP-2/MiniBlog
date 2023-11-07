using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Services;
using MiniBlog.Stores;
using Xunit;

namespace MiniBlogTest.ServiceTest;

public class ArticleServiceTest
{
    [Fact]
    public void Should_create_article_when_invoke_CreateArticle_given_input_article()
    {
        // given
        var newArticle = new Article("Jerry", "Let's code", "c#");
        var articleStore = new ArticleStore();
        var articleCountBeforeAddNewOne = articleStore.Articles.Count;
        var userStore = new UserStore();
        var articleService = new ArticleService(articleStore, userStore);

        // when
        var addedArticle = articleService.CreateArticle(newArticle);

        // then
        Assert.Equal(articleCountBeforeAddNewOne + 1, articleStore.Articles.Count);
        Assert.Equal(newArticle.Title, addedArticle.Title);
        Assert.Equal(newArticle.Content, addedArticle.Content);
        Assert.Equal(newArticle.UserName, addedArticle.UserName);
    }
}
