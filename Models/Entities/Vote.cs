namespace CodeHub.Models.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int Value { get; set; }   // +1 veya -1

        public int CommentId { get; set; }
        public Comment? Comment { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
