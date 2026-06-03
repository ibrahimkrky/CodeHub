using System.ComponentModel.DataAnnotations;

namespace CodeHub.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Koleksiyon adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        public ICollection<CollectionSnippet> CollectionSnippets { get; set; } = new List<CollectionSnippet>();
    }
}
