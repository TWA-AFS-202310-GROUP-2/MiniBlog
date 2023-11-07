using MongoDB.Driver;
using MiniBlog.Model;
using System.Security.Cryptography;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MiniBlog.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IMongoCollection<Article> articleCollection;
        public ArticleRepository(IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase("MiniBlog");
            articleCollection = mongoDatabase.GetCollection<Article>(Article.ArticleCollectionName);
        }

        public async Task<Article> CreateArticle(Article article)
        {
            await articleCollection.InsertOneAsync(article);
            return article;
        }

        public async Task<Article> GetById(Guid id)
        {
            var foundArticle = await articleCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
            return foundArticle;
        }

        public async Task<List<Article>> GetAll()
        {
            var articles = await articleCollection.Find(_ => true).ToListAsync();
            return articles;
        }
    }
}