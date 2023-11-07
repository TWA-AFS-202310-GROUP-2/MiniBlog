using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Services;
using MiniBlog.Stores;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MiniBlogTest.ServiceTest;

public class ArticleServiceTest
{
    [Fact]
    public async Task Should_create_article_when_invoke_CreateArticle_given_input_article()
    {
        // given
        var newArticle = new Article("Jerry", "Let's code", "c#");
        var newUser = new User(newArticle.UserName);
        var mockArticleRepository = new Mock<IArticleRepository>();
        mockArticleRepository.Setup(r => r.CreateArticle(newArticle)).Returns(Task.FromResult(newArticle));

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(r => r.GetByName(newArticle.UserName)).Returns(Task.FromResult<User>(null));
        //mockUserRepository.Setup(r => r.Create(newUser)).Returns(Task.FromResult(newUser));

        var articleService = new ArticleService(mockArticleRepository.Object, mockUserRepository.Object);

        // when
        var addedArticle = await articleService.CreateArticle(newArticle);

        // then
        //Assert.Null(await Task.FromResult<User>(null));
        Assert.Equal(newArticle.Title, addedArticle.Title);
        Assert.Equal(newArticle.Content, addedArticle.Content);
        Assert.Equal(newArticle.UserName, addedArticle.UserName);

        mockArticleRepository.Verify(m => m.CreateArticle(newArticle));
        mockUserRepository.Verify(m => m.GetByName(newArticle.UserName));
    }
}
