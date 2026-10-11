namespace MediaWebApp.Models;

public class SearchResults
{
    public string Query { get; set; } = string.Empty;
    public List<MediaEntry> Media { get; set; } = [];
    public List<MediaList> Lists { get; set; } = [];
    public List<UserSearchResult> Users { get; set; } = [];
}

public class UserSearchResult
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
