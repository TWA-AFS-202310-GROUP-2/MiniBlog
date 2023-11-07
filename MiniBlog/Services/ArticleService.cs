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
    private readonly IArticleRepository articleRepository = null!;

    public ArticleService(ArticleStore articleStore, UserStore userStore, IArticleRepository articleRepository)
    {
        this.articleStore = articleStore;
        this.userStore = userStore;
        this.articleRepository = articleRepository;
        this.userRepository = userRepository;
    }

    public async Task<Article?> CreateArticle(Article article)
    {
        if (article.UserName != null)
        {
            if (await userRepository.GetUser(article.UserName) == null)
            {
                await userRepository.CreateUser(new User(article.UserName));
            }
        }

        return await this.articleRepository.CreateArticleAsync(article);
    }

    public async Task<List<Article>> GetAll()
    {
        return await articleRepository.GetArticlesAsync();
    }

    public async Task<Article?> GetById(string id)
    {
        return await articleRepository.GetById(id);
    }

}
