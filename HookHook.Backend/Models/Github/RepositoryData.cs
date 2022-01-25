namespace HookHook.Backend.Models.Github
{
	public class RepositoryData
	{
		public string? RepositoryName { get; set; }
		public string? Description { get; set; }
		public string? UserName { get; set; }
        public bool Private { get; set; }

        public RepositoryData(string name, string desc, string user, bool isPrivate)
        {
            RepositoryName = name;
            Description = desc;
            UserName = user;
            Private = isPrivate;
        }

		// cf: https://docs.github.com/en/rest/reference/repos#get-a-repository
	}
}
