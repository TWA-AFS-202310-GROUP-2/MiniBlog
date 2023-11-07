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
    private IArticleRepository object1;
    private IUserRepository object2;

    public ArticleService(IArticleRepository object1, IUserRepository object2)
    {
        this.object1 = object1;
        this.object2 = object2;
    }

    public ArticleService(ArticleStore articleStore, UserStore userStore, IArticleRepository articleRepository)
    {
        this.articleStore = articleStore;
        this.userStore = userStore;
        this.articleRepository = articleRepository;
    }

    public async Task<Article?> CreateArticle(Article article)
    {
        return await articleRepository.CreateArticle(article);
    }

    public async Task<List<Article>> GetAll()
    {
        return await articleRepository.GetArticles();
    }

    public async Task<Article>? GetById(string id)
    {
        return await articleRepository.GetById(id);
    }
}
