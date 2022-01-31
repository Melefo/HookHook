
namespace HookHook.Backend.Models
{
    /// <summary>
    /// Client informations
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client class constructor
        /// </summary>
        /// <param name="host">Client address</param>
        public Client(string host) =>
            Host = host;

        /// <summary>
        /// Client address
        /// </summary>
        public string Host { get; set; }
    }

    /// <summary>
    /// Action informations
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Action class constructore
        /// </summary>
        /// <param name="name"></param>
        /// <param description="description"></param>
        public Action(string name, string description) 
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Action Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Action Description
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Reaction information
    /// </summary>
    public class Reaction
    {
        /// <summary>
        /// Reaction class constructore
        /// </summary>
        /// <param name="name"></param>
        /// <param description="description"></param>
        public Reaction(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Reaction Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reaction Description
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Service class constructor
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Service class constructore
        /// </summary>
        /// <param name="name"></param>
        public Service(string name) =>
            Name = name;

        /// <summary>
        /// Service Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Service List of Actions
        /// </summary>
        List<Action> Actions { get; set; } = new();

        /// <summary>
        /// Service List of Actions
        /// </summary>
        List<Reaction> Reactions { get; set; } = new();
    }

    /// <summary>
    /// Server informations
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Server constructor
        /// </summary>
        public Server()
        {
            CurrentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Posix Epoch when class created
        /// </summary>
        public long CurrentTime { get; set; }
        /// <summary>
        /// List of available services
        /// </summary>
        public List<Service> Services { get; set; } = new();
    }

    /// <summary>
    /// Model for /about.json route
    /// </summary>
    public class About
    {
        /// <summary>
        /// About model constructor
        /// </summary>
        /// <param name="host">Client address</param>
        public About(string host) =>
            Client = new(host);

        /// <summary>
        /// Client informations
        /// </summary>
        public Client Client { get; set; }
        /// <summary>
        /// Server informations
        /// </summary>
        public Server Server { get; set; } = new();
    }
}