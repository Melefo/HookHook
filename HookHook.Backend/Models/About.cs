
using HookHook.Backend.Attributes;
using HookHook.Backend.Entities;
using System.Reflection;

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
        public Service(string name, List<Type> areas)
        {
            Name = name;
            var iaction = typeof(IAction).GetTypeInfo();
            var ireaction = typeof(IReaction).GetTypeInfo();
            var actionList = areas.Where(x => x.GetInterfaces().Contains(iaction) && !x.IsAbstract && !x.IsInterface);
            var reactionList = areas.Where(x => x.GetInterfaces().Contains(ireaction) && !x.IsAbstract && !x.IsInterface);

            foreach (var action in actionList)
            {
                Actions.Add(new Action(action.Name, action.GetCustomAttribute<ServiceAttribute>()!.Description));
            }

            foreach (var reaction in reactionList)
            {
                Reactions.Add(new Reaction(reaction.Name, reaction.GetCustomAttribute<ServiceAttribute>()!.Description));
            }
        }

        /// <summary>
        /// Service Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Service List of Actions
        /// </summary>
        public List<Action> Actions { get; set; } = new();

        /// <summary>
        /// Service List of Actions
        /// </summary>
        public List<Reaction> Reactions { get; set; } = new();
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

            var services = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);
            var groupedServices = services.GroupBy(x => x.GetCustomAttribute<ServiceAttribute>()!.Name).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var service in groupedServices)
            {
                Services.Add(new Service(service.Key, service.Value));
            }
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
        public About(string host)
        {
            Client = new(host);
        }

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