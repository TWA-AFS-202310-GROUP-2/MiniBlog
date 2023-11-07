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
        private readonly ArticleRepository articleRepository = null!;
        private readonly UserStore userStore = null!;

        public ArticleService(ArticleRepository articleRepository, UserStore userStore)
        {
            this.articleRepository = articleRepository;
            this.userStore = userStore;
        }

        public async Task<Article> CreateArticleService(Article article)
        {
            if (article.UserName != null)
            {
                if (!userStore.Users.Exists(_ => article.UserName == _.Name))
                {
                    userStore.Users.Add(new User(article.UserName));
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