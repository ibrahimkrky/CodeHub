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
            var rows = await _db.Users
                .Select(u => new LeaderboardRow
                {
                    DisplayName = u.DisplayName ?? u.UserName!,
                    SnippetCount = u.Snippets.Count,
                    TotalLikes = u.Snippets.Sum(s => (int?)s.LikeCount) ?? 0,
                    Score = u.Score
                })
                .OrderByDescending(r => r.Score)
                .ToListAsync();
            return View(rows);
        }
    }
}
