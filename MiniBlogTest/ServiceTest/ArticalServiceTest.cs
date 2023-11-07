using Xunit;
using MiniBlog.Services;
using MiniBlog.Stores;
using MiniBlog.Model;
using MiniBlog.Repositories;
using System;
using System.Threading.Tasks;
using Moq;
using MongoDB.Driver;
using System.Collections.Generic;

namespace MiniBlogTest.ServiceTest
{
    public class ArticleServiceTest
    {
        private readonly Mock<IArticleRepository> mockArticleRepository;
        private readonly ArticleService articleService;
        private readonly UserStore userStore;

        public ArticleServiceTest()
        {
            mockArticleRepository = new Mock<IArticleRepository>();
            userStore = new UserStore(new List<User>());
            articleService = new ArticleService(mockArticleRepository.Object, userStore);
        }

        [Fact]
        public async Task ShouldCreateArticleAndRegisterUserCorrect_WhenCreateArticleService_GivenArticle()
        {
            // Arrange
            var articleToCreate = new Article("test", "test", "test");
            mockArticleRepository.Setup(repo => repo.CreateArticle(It.IsAny<Article>()))
                                 .Callback<Article>(a => a.Id = Guid.NewGuid())
                                 .ReturnsAsync((Article a) => a);

            // Act
            var createdArticle = await articleService.CreateArticleService(articleToCreate);

            // Assert
            Assert.NotNull(createdArticle.Id);
            Assert.Equal("test", createdArticle.UserName);
            Assert.Equal("test", createdArticle.Title);
            Assert.Equal("test", createdArticle.Content);
            mockArticleRepository.Verify(repo => repo.CreateArticle(It.IsAny<Article>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetArticleCorrect_WhenGetById_GivenExistingArticleId()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var expectedArticle = new Article("test", "test", "test") { Id = articleId };
            mockArticleRepository.Setup(repo => repo.GetById(articleId))
                                 .ReturnsAsync(expectedArticle);

            // Act
            var foundArticle = await articleService.GetById(articleId);

            // Assert
            Assert.Equal(expectedArticle, foundArticle);
            mockArticleRepository.Verify(repo => repo.GetById(articleId), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnNull_WhenGetById_GivenNonExistingArticleId()
        {
            // Arrange
            var nonExistingArticleId = Guid.NewGuid();
            mockArticleRepository.Setup(repo => repo.GetById(nonExistingArticleId))
                                 .ReturnsAsync((Article)null);

            // Act
            var foundArticle = await articleService.GetById(nonExistingArticleId);

            // Assert
            Assert.Null(foundArticle);
            mockArticleRepository.Verify(repo => repo.GetById(nonExistingArticleId), Times.Once);
        }
    }
}
