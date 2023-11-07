using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniBlog.Model;
using MiniBlog.Services;
using MiniBlog.Stores;

namespace MiniBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleStore articleStore = null!;
        private readonly UserStore userStore = null!;
        private readonly ArticleService articleService = null!;
        private readonly UserService userService = null!;

        public ArticleController(ArticleStore articleStore, UserStore userStore, ArticleService articleService, UserService userService)
        {
            this.articleStore = articleStore;
            this.userStore = userStore;
            this.articleService = articleService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<List<Article>> List()
        {
            return await articleService.GetAll();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article)
        {
            var addedArticle = await articleService.CreateArticle(article);

            return CreatedAtAction(nameof(GetById), new { id = article.Id }, addedArticle);
        }

        [HttpGet("{id}")]
        public Task<Article>? GetById(string id)
        {
            return articleService.GetById(id);
        }
    }
}
