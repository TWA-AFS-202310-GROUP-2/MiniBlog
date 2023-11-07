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
        private readonly ArticleService articleService = null!;

        public ArticleController(ArticleService articleService)
        {
            this.articleService = articleService;
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
        public async Task<Article?> GetById(string id)
        {
            return await articleService.GetById(id);
        }
    }
}
