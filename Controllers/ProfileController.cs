using CodeHub.Data;
using CodeHub.Models.Entities;
using CodeHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeHub.Controllers
{
    [Authorize] // Profil sayfası giriş ister
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(ApplicationDbContext db,
                                 UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Profil ana sayfası: bilgiler + snippet'ler + review'lar
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var mySnippets = await _db.Snippets
                .Where(s => s.AuthorId == user.Id)
                .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var myReviews = await _db.Reviews
                .Where(r => r.AuthorId == user.Id)
                .Include(r => r.Comments)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new ProfileViewModel
            {
                DisplayName = user.DisplayName ?? user.UserName!,
                Email = user.Email!,
                Score = user.Score,
                JoinedAt = user.JoinedAt,
                MySnippets = mySnippets,
                MyReviews = myReviews,
                TotalLikes = mySnippets.Sum(s => s.LikeCount),
                TotalViews = mySnippets.Sum(s => s.ViewCount)
            };
            return View(vm);
        }

        // Bilgi düzenleme sayfası (GET)
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var vm = new EditProfileViewModel
            {
                DisplayName = user.DisplayName ?? "",
                Email = user.Email!
            };
            return View(vm);
        }

        // Bilgi düzenleme (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.DisplayName = model.DisplayName;

            // E-posta değiştiyse UserName'i de güncelle (giriş e-posta ile yapılıyor)
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
                user.NormalizedUserName = _userManager.NormalizeName(model.Email);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Info"] = "Bilgilerin güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        // Parola değiştirme sayfası (GET)
        public IActionResult ChangePassword() => View();

        // Parola değiştirme (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _userManager.ChangePasswordAsync(
                user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Info"] = "Parolan başarıyla değiştirildi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
    }
}
