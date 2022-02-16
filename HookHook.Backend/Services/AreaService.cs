namespace HookHook.Backend.Services
{
    public class AreaService
    {
        private MongoService _db;

        public AreaService(MongoService db) =>
            _db = db;

        public async Task Trigger(string userId)
        {
            var user = _db.GetUser(userId);
            if (user == null)
                return;
            foreach (var area in user.Areas)
                await area.Launch(user);
        }
    }
}