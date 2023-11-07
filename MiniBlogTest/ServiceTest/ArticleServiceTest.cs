using System;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Services;
using MiniBlog.Stores;
using Moq;
using System.Collections.Generic;
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
    public async Task TestCreateArticle_ReturnCreatedUserObject_GivenUserDTO()
    {
        var articleToCreate = new Article("goood", "test", "test");
        var user = new User("goood");
        mockArticleRepository.Setup(ma=>ma.CreateArticle(articleToCreate)).Returns(Task.FromResult(articleToCreate));
        mockUserRepository.Setup(mu=>mu.GetUser("some")).Returns(Task.FromResult(user));
        mockUserRepository.Setup(mu=>mu.CreateUser(user)).Returns(Task.FromResult(user));

        var createdArticle = await articleService.CreateArticle(articleToCreate);

        Assert.NotNull(createdArticle.Id);
        Assert.Equal("goood", createdArticle.UserName);
        Assert.Equal("test", createdArticle.Title);
        Assert.Equal("test", createdArticle.Content);
        mockArticleRepository.Verify(repo => repo.CreateArticle(It.IsAny<Article>()), Times.Once);
        //mockUserRepository.Verify(repo => repo.CreateUser(user), Times.Once);

    }

    [Fact]
    public async Task TestGetById_ReturnMatchedArticleObject_GivenUserId()
    {
        var articleToCreate = new Article("goood", "test", "test");
        var user = new User("goood");
       
        mockArticleRepository.Setup(ma => ma.GetArticle(articleToCreate.Id)).Returns(Task.FromResult(articleToCreate));

        var articleById = await articleService.GetById(articleToCreate.Id);

        Assert.NotNull(articleById);
        Assert.Equal(articleToCreate, articleById);
    }

    [Fact]
    public async Task TestGetArticles_ReturnArticleObjects()
    {
        var articleToCreate = new Article("goood", "test", "test");
        var user = new User("goood");

        mockArticleRepository.Setup(ma => ma.GetArticles()).Returns(Task.FromResult<List<Article>>(null));

        var articleById = await articleService.GetAll();

        Assert.Null(articleById);
    }

}
