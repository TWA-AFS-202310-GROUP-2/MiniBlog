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
    private readonly ArticleStore articleStore = null!;
    private readonly UserStore userStore = null!;

    public ArticleServiceTest()
    {
        mockArticleRepository = new Mock<IArticleRepository>();
        mockUserRepository = new Mock<IUserRepository>();

        articleService = new ArticleService(articleStore, userStore, mockArticleRepository.Object, mockUserRepository.Object);
    }

    [Fact]
    public async Task Should_create_article_when_invoke_CreateArticle_given_input_articleAsync()
    {
        // given
        var newArticle = new Article("Jerry", "Let's code", "c#");

        mockArticleRepository.Setup(r => r.CreateArticle(It.IsAny<Article>())).Callback<Article>(article => article.Id = Guid.NewGuid())
            .ReturnsAsync((Article article) => article);
        mockUserRepository.Setup(r => r.GetOneUser(It.IsAny<string>())).ReturnsAsync((User)null);
        mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>())).ReturnsAsync((User user) => user);

        // when
        var addedArticle = await articleService.CreateArticle(newArticle);

        // then
        Assert.Equal(newArticle.Title, addedArticle.Title);
        Assert.Equal(newArticle.Content, addedArticle.Content);
        Assert.Equal(newArticle.UserName, addedArticle.UserName);
    }

    [Fact]
    public async Task Should_return_article_when_GetById_given_valid_article_Id()
    {
        // given
        var id = Guid.NewGuid();
        var proposedArticle = new Article("Ash", "relax", "c#");
        mockArticleRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(proposedArticle);
        // when
        var resultArticle = await articleService.GetById(id);
        // then
        Assert.Equal(proposedArticle, resultArticle);
    }
}
