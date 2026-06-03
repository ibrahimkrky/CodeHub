namespace CodeHub.Models.Entities
{
    public class Star
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int SnippetId { get; set; }
        public Snippet? Snippet { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
