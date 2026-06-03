using CodeHub.Data;
using CodeHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // sort: "new" (varsayılan), "comments", "unsolved", "solved"
        public async Task<IActionResult> Index(string sort = "new")
        {
            var reviews = await _db.Reviews
                .Include(r => r.Author)
                .Include(r => r.Comments)
                .ToListAsync();

            // Hesaplanan alanlara göre bellekte sırala
            reviews = sort switch
            {
                "comments" => reviews.OrderByDescending(r => r.Comments.Count)
                                     .ThenByDescending(r => r.CreatedAt).ToList(),
                "unsolved" => reviews.OrderBy(r => r.Comments.Any(c => c.IsSolution)) // çözülmemişler önce
                                     .ThenByDescending(r => r.CreatedAt).ToList(),
                "solved"   => reviews.OrderByDescending(r => r.Comments.Any(c => c.IsSolution)) // çözülenler önce
                                     .ThenByDescending(r => r.CreatedAt).ToList(),
                _          => reviews.OrderByDescending(r => r.CreatedAt).ToList() // "new"
            };

            ViewBag.CurrentSort = sort;
            return View(reviews);
        }

        public async Task<IActionResult> Details(int id)
        {
            var review = await _db.Reviews
                .Include(r => r.Author)
                .Include(r => r.Comments).ThenInclude(c => c.Author)
                .Include(r => r.Comments).ThenInclude(c => c.Votes)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            // Yorumları oy puanına göre sırala (en faydalı üstte).
            // AsNoTracking olduğu için navigation atama güvenli.
            review.Comments = review.Comments
                .OrderByDescending(c => c.IsSolution)
                .ThenByDescending(c => c.Votes.Sum(v => v.Value))
                .ToList();

            return View(review);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create() => View(new Review());

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review model)
        {
            if (!ModelState.IsValid) return View(model);

            model.AuthorId = _userManager.GetUserId(User)!;
            _db.Reviews.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // Kod incelemesini düzenleme sayfası (GET)
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            // Sadece incelemeyi açan kişi düzenleyebilir
            if (review.AuthorId != _userManager.GetUserId(User)) return Forbid();
            return View(review);
        }

        // Kod incelemesini düzenleme (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review model)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();
            if (review.AuthorId != _userManager.GetUserId(User)) return Forbid();

            if (!ModelState.IsValid) return View(model);

            // Sadece içerik alanlarını güncelle (yazar, tarih, yorumlar korunur)
            review.Title = model.Title;
            review.Code = model.Code;
            review.Problem = model.Problem;
            review.Language = model.Language;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = review.Id });
        }

        // Kod incelemesini sil (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _db.Reviews
                .Include(r => r.Comments).ThenInclude(c => c.Votes)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();
            if (review.AuthorId != _userManager.GetUserId(User)) return Forbid();

            // Önce bağlı oyları ve yorumları temizle, sonra incelemeyi sil
            foreach (var c in review.Comments)
                _db.Votes.RemoveRange(c.Votes);
            _db.Comments.RemoveRange(review.Comments);
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int reviewId, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return RedirectToAction(nameof(Details), new { id = reviewId });

            _db.Comments.Add(new Comment
            {
                ReviewId = reviewId,
                AuthorId = _userManager.GetUserId(User)!,
                Body = body.Trim()
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = reviewId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int commentId, int value)
        {
            value = value >= 0 ? 1 : -1;
            var userId = _userManager.GetUserId(User)!;

            var comment = await _db.Comments.Include(c => c.Votes)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null) return NotFound();

            var existing = comment.Votes.FirstOrDefault(v => v.UserId == userId);

            int myVote; // kullanıcının son oy yönü: 1, -1 veya 0
            if (existing == null)
            {
                // Hiç oy yok -> yeni oy ekle
                _db.Votes.Add(new Vote { CommentId = commentId, UserId = userId, Value = value });
                myVote = value;
            }
            else if (existing.Value == value)
            {
                // Aynı yöne tekrar basıldı -> oyu geri çek (sil)
                _db.Votes.Remove(existing);
                myVote = 0;
            }
            else
            {
                // Ters yöne basıldı -> oyu değiştir
                existing.Value = value;
                myVote = value;
            }

            await _db.SaveChangesAsync();

            int total = await _db.Votes.Where(v => v.CommentId == commentId).SumAsync(v => v.Value);
            return Json(new { total, myVote });
        }

        // Soru sahibi bir yorumu çözüm olarak işaretler / işareti kaldırır (toggle)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkSolution(int commentId)
        {
            var comment = await _db.Comments.Include(c => c.Review)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null) return NotFound();

            // Sadece review sahibi işaretleyebilir
            if (comment.Review!.AuthorId != _userManager.GetUserId(User)) return Forbid();

            if (comment.IsSolution)
            {
                // Zaten çözüm -> işareti kaldır (iptal)
                comment.IsSolution = false;
            }
            else
            {
                // Aynı review'daki diğer çözümleri sıfırla, bunu işaretle
                var siblings = await _db.Comments.Where(c => c.ReviewId == comment.ReviewId).ToListAsync();
                foreach (var c in siblings) c.IsSolution = false;
                comment.IsSolution = true;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = comment.ReviewId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int commentId, string body)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return NotFound();

            // Sadece yorum sahibi düzenleyebilir
            if (comment.AuthorId != _userManager.GetUserId(User)) return Forbid();

            if (!string.IsNullOrWhiteSpace(body))
            {
                comment.Body = body.Trim();
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = comment.ReviewId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _db.Comments.Include(c => c.Votes)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null) return NotFound();

            // Sadece yorum sahibi silebilir
            if (comment.AuthorId != _userManager.GetUserId(User)) return Forbid();

            int reviewId = comment.ReviewId;
            _db.Votes.RemoveRange(comment.Votes);   // önce oyları temizle
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = reviewId });
        }
    }
}
