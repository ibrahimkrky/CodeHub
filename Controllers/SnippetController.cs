using CodeHub.Data;
using CodeHub.Models.Entities;
using CodeHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    public class SnippetController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SnippetController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private static readonly string[] Languages =
            { "C#", "JavaScript", "SQL", "CSS", "HTML", "Python", "React", "ASP.NET", "TypeScript", "Java" };

        public async Task<IActionResult> Details(int id)
        {
            var snippet = await _db.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (snippet == null) return NotFound();

            // Görüntülenme sayısını artır
            snippet.ViewCount++;
            await _db.SaveChangesAsync();

            // Giriş yapmış kullanıcının koleksiyonlarını "koleksiyona ekle" menüsü için gönder
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                ViewBag.MyCollections = await _db.Collections
                    .Where(c => c.OwnerId == userId)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            else
            {
                ViewBag.MyCollections = new List<Collection>();
            }

            return View(snippet);
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Languages = new SelectList(Languages);
            return View(new SnippetFormViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SnippetFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = new SelectList(Languages, model.Language);
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var snippet = new Snippet
            {
                Title = model.Title,
                Code = model.Code,
                Description = model.Description,
                Language = model.Language,
                AuthorId = userId
            };

            await ApplyTagsAsync(snippet, model.TagsCsv);

            _db.Snippets.Add(snippet);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = snippet.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var snippet = await _db.Snippets
                .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (snippet == null) return NotFound();

            // Sahiplik kontrolü
            if (snippet.AuthorId != _userManager.GetUserId(User)) return Forbid();

            var vm = new SnippetFormViewModel
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Description = snippet.Description,
                Language = snippet.Language,
                TagsCsv = string.Join(", ", snippet.SnippetTags.Select(st => st.Tag!.Name))
            };
            ViewBag.Languages = new SelectList(Languages, snippet.Language);
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SnippetFormViewModel model)
        {
            var snippet = await _db.Snippets
                .Include(s => s.SnippetTags)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (snippet == null) return NotFound();
            if (snippet.AuthorId != _userManager.GetUserId(User)) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Languages = new SelectList(Languages, model.Language);
                return View(model);
            }

            snippet.Title = model.Title;
            snippet.Code = model.Code;
            snippet.Description = model.Description;
            snippet.Language = model.Language;

            // Etiketleri sıfırla ve yeniden uygula
            _db.SnippetTags.RemoveRange(snippet.SnippetTags);
            await ApplyTagsAsync(snippet, model.TagsCsv);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = snippet.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var snippet = await _db.Snippets.FindAsync(id);
            if (snippet == null) return NotFound();
            if (snippet.AuthorId != _userManager.GetUserId(User)) return Forbid();

            _db.Snippets.Remove(snippet);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // Beğeni ekle/kaldır (AJAX, sayfa yenilenmeden)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Star(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var snippet = await _db.Snippets.FindAsync(id);
            if (snippet == null) return NotFound();

            var existing = await _db.Stars
                .FirstOrDefaultAsync(s => s.SnippetId == id && s.UserId == userId);

            bool liked;
            if (existing == null)
            {
                _db.Stars.Add(new Star { SnippetId = id, UserId = userId });
                snippet.LikeCount++;
                liked = true;
            }
            else
            {
                _db.Stars.Remove(existing);
                snippet.LikeCount = Math.Max(0, snippet.LikeCount - 1);
                liked = false;
            }

            await _db.SaveChangesAsync();
            return Json(new { liked, likeCount = snippet.LikeCount });
        }

        // Yardımcı: "C#, LINQ" -> SnippetTag kayıtları (var olan tag'i bul, yoksa oluştur)
        private async Task ApplyTagsAsync(Snippet snippet, string? tagsCsv)
        {
            if (string.IsNullOrWhiteSpace(tagsCsv)) return;

            var names = tagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                               .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var name in names)
            {
                var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name);
                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    _db.Tags.Add(tag);
                    await _db.SaveChangesAsync();
                }
                snippet.SnippetTags.Add(new SnippetTag { TagId = tag.Id });
            }
        }
    }
}
