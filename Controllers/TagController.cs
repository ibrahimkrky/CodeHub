using CodeHub.Data;
using CodeHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TagController(ApplicationDbContext db) => _db = db;

        // Tüm etiketler + her birine bağlı snippet sayısı (Many-to-Many)
        public async Task<IActionResult> Index()
        {
            var rows = await _db.Tags
                .Select(t => new TagIndexRow
                {
                    Id = t.Id,
                    Name = t.Name,
                    SnippetCount = t.SnippetTags.Count
                })
                .OrderBy(t => t.Name)
                .ToListAsync();
            return View(rows);
        }

        // Belirli bir etikete ait snippet'leri listele
        public async Task<IActionResult> Snippets(int id)
        {
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            var snippets = await _db.Snippets
                .Where(s => s.SnippetTags.Any(st => st.TagId == id))
                .Include(s => s.Author)
                .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            ViewBag.TagName = tag.Name;
            return View(snippets);
        }
    }
}
