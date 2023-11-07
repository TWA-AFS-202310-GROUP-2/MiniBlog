using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;

namespace MiniBlog.Services;

public class ArticleService
{
    private readonly ArticleStore articleStore = null!;
    private readonly UserStore userStore = null!;
    private readonly ArticleRepository articleRepository = null!;

    public ArticleService(ArticleStore articleStore, UserStore userStore, ArticleRepository articleRepository)
    {
        this.articleStore = articleStore;
        this.userStore = userStore;
        this.articleRepository = articleRepository;
    }

    public Article? CreateArticle(Article article)
    {
        if (article.UserName != null)
        {
            if (!userStore.Users.Exists(_ => article.UserName == _.Name))
            {
                userStore.Users.Add(new User(article.UserName));
            }

            articleStore.Articles.Add(article);
        }

        return articleStore.Articles.Find(articleExisted => articleExisted.Title == article.Title);
    }

    public async Task<List<Article>> GetAll()
    {
        return await articleRepository.GetAllArticles();
    }

    public Article? GetById(Guid id)
    {
        return articleStore.Articles.FirstOrDefault(article => article.Id == id.ToString());
    }
}