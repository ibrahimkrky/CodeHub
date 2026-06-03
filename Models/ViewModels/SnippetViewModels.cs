using CodeHub.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Snippet> Snippets { get; set; } = new();
        public List<Tag> PopularTags { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public string CurrentSort { get; set; } = "new";
    }

    public class SnippetFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kod alanı boş bırakılamaz.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Dil seçimi zorunludur.")]
        public string Language { get; set; } = string.Empty;

        // Virgülle ayrılmış etiketler: "C#, LINQ"
        public string? TagsCsv { get; set; }
    }
}
