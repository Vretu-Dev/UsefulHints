using LabApi.Features.Wrappers;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Maps
    {
        private static CoroutineHandle broadcastCoroutine;

        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        }

        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
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

            if (Round.IsRoundEnded)
                yield break;

            foreach (var player in Player.ReadyList.Where(p => p.IsAlive))
            {
                player.SendBroadcast(UsefulHints.Instance.Config.BroadcastWarningLcz, 7);
            }
        }
    }
}