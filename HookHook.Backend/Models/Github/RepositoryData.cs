namespace HookHook.Backend.Models.Github
{
	public class RepositoryData
	{
		public string? RepositoryName { get; set; }
		public string? UserName { get; set; }

		// * probably a list of commits, issues, branches, etc...
		// cf: https://docs.github.com/en/rest/reference/repos#get-a-repository
	}
}
