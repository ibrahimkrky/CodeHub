using CodeHub.Data;
using CodeHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) => _db = db;

        // Dashboard: sıralanabilir snippet listesi
        // sort: "new" (varsayılan), "old", "popular", "views"
        public async Task<IActionResult> Index(string sort = "new")
        {
            IQueryable<CodeHub.Models.Entities.Snippet> query = _db.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags).ThenInclude(st => st.Tag);

            // Seçilen sıralamaya göre LINQ sırala
            query = sort switch
            {
                "old"     => query.OrderBy(s => s.CreatedAt),
                "popular" => query.OrderByDescending(s => s.LikeCount),
                "views"   => query.OrderByDescending(s => s.ViewCount),
                _         => query.OrderByDescending(s => s.CreatedAt) // "new"
            };

            var snippets = await query.Take(12).ToListAsync();

            var popularTags = await _db.Tags
                .OrderByDescending(t => t.SnippetTags.Count)
                .Take(8)
                .ToListAsync();

            // Dashboard'da görünen snippet'lerde geçen benzersiz diller (filtre butonları için)
            var languages = snippets
                .Select(s => s.Language)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            var vm = new DashboardViewModel
            {
                Snippets = snippets,
                PopularTags = popularTags,
                Languages = languages,
                CurrentSort = sort
            };
            return View(vm);
        }

        public IActionResult Error() => View();
    }
}
