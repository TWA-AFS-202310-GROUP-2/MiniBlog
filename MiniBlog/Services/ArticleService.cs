using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Services
{
    public class ArticleService
    {
        private readonly IArticleRepository articleRepository = null!;
        private readonly IUserRepository userRepository = null!;

        public ArticleService(IArticleRepository articleRepository, IUserRepository userRepository)
        {
            this.articleRepository = articleRepository;
            this.userRepository = userRepository;
        }

        public async Task<Article> CreateArticleService(Article article)
        {
            if (article.UserName != null)
            {
                if (await userRepository.GetUserByName(article.UserName) != null)
                {
                    await userRepository.CreateUser(new User { Name = article.UserName });
                }

                await articleRepository.CreateArticle(article);
            }

            return article;           
        } 

        public async Task<Article> GetById(Guid id)
        {
            var foundArticle = await articleRepository.GetById(id);
            return foundArticle;
        }

        public async Task<List<Article>> GetAll()
        {
            var articles = await articleRepository.GetAll();
            return articles;
        }
    }
}