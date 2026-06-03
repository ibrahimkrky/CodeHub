using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yorum boş olamaz.")]
        public string Body { get; set; } = string.Empty;

        public bool IsSolution { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ReviewId { get; set; }
        public Review? Review { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
