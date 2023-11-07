using Xunit;
using MiniBlog.Services;
using MiniBlog.Stores;
using MiniBlog.Model;
using System.Collections.Generic;
using System;

namespace MiniBlogTest.ServiceTest
{
    public class ArticalServiceTest
    {
        [Fact]
        public void ShouldCreateArticleAndRegisterUserCorrect_WhenCreateArticleService_GivenArticle()
        {
            var articleService = new ArticleService(new ArticleStore(), new UserStore(new List<User>()));
            var article = articleService.CreateArticleService(new Article("test", "test", "test"));
            Assert.Equal("test", article.UserName);
            Assert.Equal("test", article.Title);
            Assert.Equal("test", article.Content);
        }

        [Fact]
        public void ShouldGetArticleCorrect_WhenGetById_GivenExistingArticleId()
        {
            var articleService = new ArticleService(new ArticleStore(), new UserStore(new List<User>()));
            var article = articleService.CreateArticleService(new Article("test", "test", "test"));
            var articleId = article.Id;
            var foundArticle = articleService.GetById(articleId);
            Assert.Equal(article, foundArticle);
        }

        [Fact]
        public void ShouldReturnNull_WhenGetById_GivenNonExistingArticleId()
        {
            var articleService = new ArticleService(new ArticleStore(), new UserStore(new List<User>()));
            var article = articleService.CreateArticleService(new Article("test", "test", "test"));
            var articleId = article.Id;
            var foundArticle = articleService.GetById(Guid.NewGuid());
            Assert.Null(foundArticle);
        }
    }
}