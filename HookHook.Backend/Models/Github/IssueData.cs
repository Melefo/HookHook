namespace HookHook.Backend.Models.Github
{
	public class IssueData
	{
		public string? Name { get; set; }
		public string? MainBody { get; set; }
		public string? AuthorUrl { get; set; }

        public IssueData(string name, string body, string authorUrl)
        {
            Name = name;
            MainBody = body;
            AuthorUrl = authorUrl;
        }

		// cf: https://docs.github.com/en/rest/reference/issues#get-an-issue
	}
}
