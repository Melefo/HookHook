using System;
using FluentScheduler;

namespace HookHook.Backend.Services
{
	public class AreaService : Registry
	{
        public MongoService _mongo;

        public AreaService(MongoService mongo)
        {
            _mongo = mongo;
            Schedule(async () => await Execute()).ToRunEvery(1).Minutes();
        }

        private async Task Execute()
        {
            var users = _mongo.GetUsers();

            foreach (var user in users)
                foreach (var area in user.Areas)
                    await area.Launch(user, _mongo);
        }
	}
}