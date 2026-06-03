using Microsoft.AspNetCore.Identity;

namespace CodeHub.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public int Score { get; set; } = 0;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Snippet> Snippets { get; set; } = new List<Snippet>();
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();
        public ICollection<Star> Stars { get; set; } = new List<Star>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
