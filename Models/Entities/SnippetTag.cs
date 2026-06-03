namespace CodeHub.Models.Entities
{
    public class SnippetTag
    {
        public int SnippetId { get; set; }
        public Snippet? Snippet { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
