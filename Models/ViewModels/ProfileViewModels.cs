using CodeHub.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.ViewModels
{
    // Profil sayfasının tamamı için
    public class ProfileViewModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime JoinedAt { get; set; }

        public List<Snippet> MySnippets { get; set; } = new();
        public List<Review> MyReviews { get; set; } = new();

        public int TotalLikes { get; set; }
        public int TotalViews { get; set; }
    }

    // Bilgi düzenleme formu (ad + e-posta)
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Görünen ad zorunludur.")]
        [Display(Name = "Görünen Ad")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
    }

    // Parola değiştirme formu
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut parola zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Parola")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni parola zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola en az 6 karakter olmalı.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Parola")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Parolalar eşleşmiyor.")]
        [Display(Name = "Yeni Parola (Tekrar)")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
