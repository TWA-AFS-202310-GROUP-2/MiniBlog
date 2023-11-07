using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniBlog.Model;
namespace MiniBlog.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> CreateArticle(Article article);
        Task<Article> GetById(Guid id);
        Task<List<Article>> GetAll();
    }
}