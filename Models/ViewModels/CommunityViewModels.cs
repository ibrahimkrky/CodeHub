using CodeHub.Models.Entities;

namespace CodeHub.Models.ViewModels
{
    public class LeaderboardRow
    {
        public string DisplayName { get; set; } = string.Empty;
        public int SnippetCount { get; set; }
        public int TotalLikes { get; set; }
        public int Score { get; set; }
    }

    public class TagIndexRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SnippetCount { get; set; }
    }
}
