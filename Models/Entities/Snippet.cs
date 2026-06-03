using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.Entities
{
    public class Snippet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kod alanı boş bırakılamaz.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Dil seçimi zorunludur.")]
        [StringLength(50)]
        public string Language { get; set; } = string.Empty;

        public int LikeCount { get; set; } = 0;
        public int ViewCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }

        public ICollection<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
        public ICollection<CollectionSnippet> CollectionSnippets { get; set; } = new List<CollectionSnippet>();
        public ICollection<Star> Stars { get; set; } = new List<Star>();
    }
}
