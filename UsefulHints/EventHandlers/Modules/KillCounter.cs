using LabApi.Events.Arguments.PlayerEvents;
using System.Collections.Generic;
using Player = LabApi.Features.Wrappers.Player;

namespace UsefulHints.EventHandlers.Modules
{
    public static class KillCounter
    {
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDied;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        }

        private static void OnWaitingForPlayers()
        {
            playerKills.Clear();
        }

        private static void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                if (playerKills.ContainsKey(ev.Attacker))
                {
                    playerKills[ev.Attacker]++;
                }
                else
                {
                    playerKills[ev.Attacker] = 1;
                }

                if (!ev.Attacker.IsHost)
                {
                    ev.Attacker.SendHint(string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[ev.Attacker]), 4);
                }
            }
        }
    }
}
