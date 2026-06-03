using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kod alanı boş bırakılamaz.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Problem { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
