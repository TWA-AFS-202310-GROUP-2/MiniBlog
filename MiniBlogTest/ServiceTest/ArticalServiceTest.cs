using Xunit;
using MiniBlog.Services;
using MiniBlog.Stores;
using MiniBlog.Model;
using System.Collections.Generic;
using MiniBlog.Repositories;
using System;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Moq;

namespace MiniBlogTest.ServiceTest
{
    public class ArticalServiceTest
    {
        private readonly ArticleRepository articleRepository;

        public ArticalServiceTest()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            articleRepository = new ArticleRepository(mongoClient);
        }

        [Fact]
        public async Task ShouldCreateArticleAndRegisterUserCorrect_WhenCreateArticleService_GivenArticle()
        {
            var articleService = new ArticleService(articleRepository, new UserStore(new List<User>()));
            var article = await articleService.CreateArticleService(new Article("test", "test", "test"));
            Assert.Equal("test", article.UserName);
            Assert.Equal("test", article.Title);
            Assert.Equal("test", article.Content);
        }

        [Fact]
        public async Task ShouldGetArticleCorrect_WhenGetById_GivenExistingArticleId()
        {
            var articleService = new ArticleService(articleRepository, new UserStore(new List<User>()));
            var article = await articleService.CreateArticleService(new Article("test", "test", "test"));
            var articleId = article.Id;
            var foundArticle = await articleService.GetById(articleId);
            Assert.Equal(article, foundArticle);
        }

        [Fact]
        public async Task ShouldReturnNull_WhenGetById_GivenNonExistingArticleId()
        {
            var articleService = new ArticleService(articleRepository, new UserStore(new List<User>()));
            var article = await articleService.CreateArticleService(new Article("test", "test", "test"));
            var foundArticle = await articleService.GetById(Guid.NewGuid());
            Assert.Null(foundArticle);
        }
    }
}