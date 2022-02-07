namespace HookHook.Backend.Models.Github
{
	public class CommitData
	{
		public string? RepositoryName { get; set; }
		public string? RepositoryOwner { get; set; }
		public string? Message { get; set; }
		public string? Author { get; set; }
        // * date ?
        public string ?Date { get; set; }

        public CommitData(string repoName, string owner, string msg, string author, string date)
        {
            RepositoryName = repoName;
            RepositoryOwner = owner;
            Message = msg;
            Author = author;
            Date = date;
        }

		// https://docs.github.com/en/rest/reference/commits#list-commits
	}
}
