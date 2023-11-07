using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniBlog.Model;
using MiniBlog.Stores;
using MiniBlog.Services;

namespace MiniBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleStore articleStore = null!;
        private readonly UserStore userStore = null!;

        private readonly ArticleService articleService = null!;

        public ArticleController(ArticleStore articleStore, UserStore userStore, ArticleService articleService)
        {
            this.articleStore = articleStore;
            this.userStore = userStore;
            this.articleService = articleService;
        }

        [HttpGet]
        public List<Article> List()
        {
            Console.WriteLine(articleStore.Articles);
            return articleStore.Articles;
        }

        [HttpPost]
        public IActionResult Create(Article article)
        {
            articleService.CreateArticleService(article);
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }

        [HttpGet("{id}")]
        public Article GetById(Guid id)
        {
            var foundArticle = articleService.GetById(id);
            return foundArticle;
        }
    }
}
