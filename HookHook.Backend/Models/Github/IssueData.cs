namespace HookHook.Backend.Models.Github
{
	public class IssueData
	{
		public string? Title { get; set; }
		public string? Body { get; set; }
		public string? AuthorUrl { get; set; }
		public string? AuthorUser { get; set; }
		public string? RepositoryUrl { get; set; }

        // * don't really need these
        // public string[]? Labels {get; set;}
        // public string[]? Assignees {get; set;}

        public IssueData(string title, string body, string authorUrl, string authorUsername, string repositoryUrl)
        {
            Title = title;
            Body = body;
            AuthorUrl = authorUrl;
            AuthorUser = authorUsername;
            RepositoryUrl = repositoryUrl;
        }

		// cf: https://docs.github.com/en/rest/reference/issues#get-an-issue
	}
}
