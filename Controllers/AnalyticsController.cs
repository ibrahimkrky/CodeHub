using CodeHub.Data;
using CodeHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnalyticsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        // Grafiklerin kullanacağı JSON verisi
        [HttpGet]
        public async Task<IActionResult> Data()
        {
            var userId = _userManager.GetUserId(User)!;
            var mySnippets = await _db.Snippets
                .Where(s => s.AuthorId == userId)
                .ToListAsync();

            // Dile göre dağılım (pie)
            var byLanguage = mySnippets
                .GroupBy(s => s.Language)
                .Select(g => new { language = g.Key, count = g.Count() })
                .ToList();

            // Snippet başına görüntülenme (bar)
            var views = mySnippets
                .OrderByDescending(s => s.ViewCount)
                .Take(8)
                .Select(s => new { title = s.Title, views = s.ViewCount })
                .ToList();

            return Json(new
            {
                byLanguage,
                views,
                totalSnippets = mySnippets.Count,
                totalViews = mySnippets.Sum(s => s.ViewCount),
                totalLikes = mySnippets.Sum(s => s.LikeCount)
            });
        }
    }
}
