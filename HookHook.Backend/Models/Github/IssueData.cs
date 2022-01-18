namespace HookHook.Backend.Models.Github
{
	public class IssueData
	{
		public string? Name { get; set; }
		public string? MainBody { get; set; }
		public string? Author { get; set; }

		// cf: https://docs.github.com/en/rest/reference/issues#get-an-issue
	}
}
