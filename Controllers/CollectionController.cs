using CodeHub.Data;
using CodeHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    [Authorize] // Tüm sayfa giriş ister
    public class CollectionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CollectionController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyCollections()
        {
            var userId = _userManager.GetUserId(User)!;
            var collections = await _db.Collections
                .Where(c => c.OwnerId == userId)
                .Include(c => c.CollectionSnippets)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(collections);
        }

        // Bir koleksiyonun içindeki snippet'leri gösterir
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var collection = await _db.Collections
                .Include(c => c.CollectionSnippets).ThenInclude(cs => cs.Snippet)
                    .ThenInclude(s => s!.Author)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collection == null) return NotFound();

            // Sadece sahibi görebilsin
            if (collection.OwnerId != userId) return Forbid();

            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Koleksiyon adı boş olamaz.";
                return RedirectToAction(nameof(MyCollections));
            }

            var userId = _userManager.GetUserId(User)!;
            _db.Collections.Add(new Collection { Name = name.Trim(), OwnerId = userId });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(MyCollections));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rename(int id, string name)
        {
            var col = await _db.Collections.FindAsync(id);
            if (col == null) return NotFound();
            if (col.OwnerId != _userManager.GetUserId(User)) return Forbid();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Koleksiyon adı boş olamaz.";
                return RedirectToAction(nameof(MyCollections));
            }

            col.Name = name.Trim();
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(MyCollections));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var col = await _db.Collections
                .Include(c => c.CollectionSnippets)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (col == null) return NotFound();
            if (col.OwnerId != _userManager.GetUserId(User)) return Forbid();

            // Önce ara tablo kayıtlarını temizle, sonra koleksiyonu sil
            _db.CollectionSnippets.RemoveRange(col.CollectionSnippets);
            _db.Collections.Remove(col);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(MyCollections));
        }

        // Snippet detay sayfasından çağrılır: snippet'i bir koleksiyona ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSnippet(int collectionId, int snippetId)
        {
            var userId = _userManager.GetUserId(User)!;
            var col = await _db.Collections.FindAsync(collectionId);
            if (col == null) return NotFound();
            if (col.OwnerId != userId) return Forbid();

            bool exists = await _db.CollectionSnippets
                .AnyAsync(cs => cs.CollectionId == collectionId && cs.SnippetId == snippetId);

            if (!exists)
            {
                _db.CollectionSnippets.Add(new CollectionSnippet
                {
                    CollectionId = collectionId,
                    SnippetId = snippetId
                });
                await _db.SaveChangesAsync();
                TempData["Info"] = $"\"{col.Name}\" koleksiyonuna eklendi.";
            }
            else
            {
                TempData["Info"] = "Bu snippet zaten koleksiyonda.";
            }

            return RedirectToAction("Details", "Snippet", new { id = snippetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSnippet(int collectionId, int snippetId)
        {
            var userId = _userManager.GetUserId(User)!;
            var col = await _db.Collections.FindAsync(collectionId);
            if (col == null) return NotFound();
            if (col.OwnerId != userId) return Forbid();

            var link = await _db.CollectionSnippets
                .FirstOrDefaultAsync(cs => cs.CollectionId == collectionId && cs.SnippetId == snippetId);
            if (link != null)
            {
                _db.CollectionSnippets.Remove(link);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyCollections));
        }
    }
}
