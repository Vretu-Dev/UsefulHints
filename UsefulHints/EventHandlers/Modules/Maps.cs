using Exiled.API.Features;
using MEC;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Maps
    {
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        }
        private static void OnRoundStarted()
        {
            Timing.CallDelayed(445f, () =>
            {
                string message = UsefulHints.Instance.Config.BroadcastWarningLcz;

                foreach (var player in Player.List)
                {
                    if (player.IsAlive)
                    {
                        player.Broadcast(7, message);
                    }
                }
            });
        }
    }
}