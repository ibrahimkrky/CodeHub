using CodeHub.Data;
using CodeHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CommunityController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Leaderboard()
        {
            // Puan canlı hesaplanır: (snippet sayısı * 5) + toplam beğeni.
            // Böylece yeni snippet/beğeni anında tabloya yansır.
            var rows = await _db.Users
                .Select(u => new LeaderboardRow
                {
                    DisplayName = u.DisplayName ?? u.UserName!,
                    SnippetCount = u.Snippets.Count,
                    TotalLikes = u.Snippets.Sum(s => (int?)s.LikeCount) ?? 0,
                    Score = u.Snippets.Count * 5 + (u.Snippets.Sum(s => (int?)s.LikeCount) ?? 0)
                })
                .OrderByDescending(r => r.Score)
                .ToListAsync();
            return View(rows);
        }
    }
}
