using MiniBlog.Model;
using MiniBlog.Stores;
using System;

namespace MiniBlog.Services
{
    public class ArticleService
    {
        private readonly ArticleStore articleStore = null!;
        private readonly UserStore userStore = null!;

        public ArticleService(ArticleStore articleStore, UserStore userStore)
        {
            this.articleStore = articleStore;
            this.userStore = userStore;
        }

        public Article CreateArticleService(Article article)
        {
            article.Id = Guid.NewGuid();
            if (article.UserName != null)
            {
                if (!userStore.Users.Exists(_ => article.UserName == _.Name))
                {
                    userStore.Users.Add(new User(article.UserName));
                }

                articleStore.Articles.Add(article);
            }

            return article;           
        } 

        public Article GetById(Guid id)
        {
            var foundArticle = articleStore.Articles.Find(article => article.Id == id);
            return foundArticle;
        }
    }
}