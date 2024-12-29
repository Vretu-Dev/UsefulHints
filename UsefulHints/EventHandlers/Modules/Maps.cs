using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using System.Collections.Generic;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Maps
    {
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

            string message = UsefulHints.Instance.Config.BroadcastWarningLcz;

            foreach (var player in Player.List)
            {
                if (player.IsAlive)
                {
                    player.Broadcast(7, message);
                }
            }
        }
    }
}