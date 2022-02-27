using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using HookHook.Backend.Services;

namespace HookHook.Backend.Entities
{
    /// <summary>
    /// An AREA data saved in database
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Database User-Object Id
        /// </summary>
        //[JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private init; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; private init; }

        /// <summary>
        /// Last successful Area update
        /// </summary>
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Number of minutes between updates
        /// </summary>
        public int MinutesBetween { get; set; }

        /// <summary>
        /// Action we are waiting for
        /// </summary>
        public IAction Action { get; private init; }

        /// <summary>
        /// Reactions executed if action
        /// </summary>
        public List<IReaction> Reactions { get; private init; }

        public Area(string name, IAction action, IEnumerable<IReaction> reactions, int minutes)
        {
            Name = name;
            Action = action;
            Reactions = new(reactions);
            MinutesBetween = minutes;
        }

        public async Task Launch(User user, MongoService _db)
        {
            if (LastUpdate > DateTime.UtcNow.AddMinutes(MinutesBetween))
                return;
            LastUpdate = DateTime.UtcNow.AddMinutes(MinutesBetween);
            (var formatters, bool actionValue) = await Action.Check(user);

            if (!actionValue)
                return;

            foreach (var reaction in Reactions) {
                try
                {
                    await reaction.Execute(user, formatters!);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Source}: {e.Message}");
                    return;
                }
            }
            _db.SaveUser(user);
        }
    }
}