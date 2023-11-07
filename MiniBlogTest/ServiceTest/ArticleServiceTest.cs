using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Services;
using MiniBlog.Stores;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MiniBlogTest.ServiceTest;

public class ArticleServiceTest
{
    private readonly Mock<IArticleRepository> mockArticleRepository;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly ArticleService articleService;

    public ArticleServiceTest()
    {
        mockArticleRepository = new Mock<IArticleRepository>();
        mockUserRepository = new Mock<IUserRepository>();
        articleService = new ArticleService(mockArticleRepository.Object, mockUserRepository.Object);
    }

    [Fact]
    public async Task Should_create_article_when_invoke_CreateArticle_given_input_article()
    {
        // given
        var newArticle = new Article("Jerry", "Let's code", "c#");

        mockArticleRepository.Setup(r => r.CreateArticle(It.IsAny<Article>())).Callback<Article>(article => article.Id = Guid.NewGuid().ToString()).ReturnsAsync((Article article) => article);
        mockUserRepository.Setup(r => r.GetByName(It.IsAny<string>())).ReturnsAsync((User)null);
        mockUserRepository.Setup(r => r.Create(It.IsAny<User>())).ReturnsAsync((User user) => user);

        // when
        var addedArticle = await articleService.CreateArticle(newArticle);

        // then
        Assert.NotNull(addedArticle.Id);
        Assert.Equal(newArticle.Title, addedArticle.Title);
        Assert.Equal(newArticle.Content, addedArticle.Content);
        Assert.Equal(newArticle.UserName, addedArticle.UserName);

        mockArticleRepository.Verify(m => m.CreateArticle(It.IsAny<Article>()), Times.Once);
        mockUserRepository.Verify(m => m.GetByName(It.IsAny<string>()), Times.Once);
        mockUserRepository.Verify(m => m.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Should_get_article_when_GetById_given_valid_id()
    {
        var id = Guid.NewGuid().ToString();
        var expectedArticle = new Article("Jerry", "Let's code", "c#");

        mockArticleRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(expectedArticle);

        //when
        var article = await articleService.GetById(id);

        //then
        Assert.Equal(expectedArticle, article);
        mockArticleRepository.Verify(repo => repo.GetById(id), Times.Once);
    }
}
