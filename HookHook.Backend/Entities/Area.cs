using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

        /// <summary>
        /// Last successful Area update
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Number of minutes between updates
        /// </summary>
        public int MinutesBetween { get; set; }

        /// <summary>
        /// Action we are waiting for
        /// </summary>
        public IAction Action { get; private init; }

        /// <summary>
        /// Reaction executed if action
        /// </summary>
        public IReaction Reaction { get; private init; }

        public Area(IAction action, IReaction reaction, int minutes)
        {
            Action = action;
            Reaction = reaction;
            MinutesBetween = minutes;
        }

        public async Task Launch(User user)
        {
            (string? actionInfo, bool actionValue) = await Action.Check(user);

            if (actionValue)
            {
                await Reaction.Execute(user);
            }
    }
        }
}
