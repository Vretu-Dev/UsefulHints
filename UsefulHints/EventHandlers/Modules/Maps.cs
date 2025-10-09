using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using MEC;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Maps
    {
        private static Config Config => UsefulHints.Instance.Config;
        private static CoroutineHandle broadcastCoroutine;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        private static void OnRoundStarted()
        {
            broadcastCoroutine = Timing.RunCoroutine(DelayedBroadcastCoroutine());
        }

        private static void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(broadcastCoroutine);
        }

        private static IEnumerator<float> DelayedBroadcastCoroutine()
        {
            yield return Timing.WaitForSeconds(445f);

            if (Round.IsEnded)
                yield break;

            foreach (var player in Player.List.Where(p => p.IsAlive))
            {
                player.Broadcast(7, Config.BroadcastWarningLcz);
            }
        }
    }
}