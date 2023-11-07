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
        private readonly UserService userService = null!;
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
            await articleService.CreateArticle(article);
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }

        [HttpGet("{id}")]
        public Article? GetById(Guid id)
        {
            return articleService.GetById(id);
        }
    }
}
