namespace CodeHub.Models.Entities
{
    public class CollectionSnippet
    {
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }

        public int SnippetId { get; set; }
        public Snippet? Snippet { get; set; }
    }
}
