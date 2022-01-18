namespace HookHook.Backend.Models.Github
{
	public class CommitData
	{
		public string? RepositoryName { get; set; }
		public string? Message { get; set; }
		public string? Author { get; set; }

		// https://docs.github.com/en/rest/reference/commits#list-commits
	}
}
