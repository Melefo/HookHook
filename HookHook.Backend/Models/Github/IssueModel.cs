namespace HookHook.Backend.Models.Github
{
	public class IssueModel
	{
		public string? Title { get; set; }
		public string? Body { get; set; }

        public string[]? Labels {get; set;}
        public string[]? Assignees {get; set;}

		public string? RepositoryName { get; set; }
		public string? UserName { get; set; }

	}
}
