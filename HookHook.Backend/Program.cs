namespace HookHook.Backend
{
    /// <summary>
    /// Base class of the project
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main function starting our program
        /// </summary>
        /// <param name="args">Program args</param>
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        /// <summary>
        /// Creating a web app
        /// </summary>
        /// <param name="args">Program args</param>
        /// <returns>Program initialization abstraction</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}