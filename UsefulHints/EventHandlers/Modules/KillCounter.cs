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
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
        }
        private static void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                {
                    playerKills[killer]++;
                }
                else
                {
                    playerKills[killer] = 1;
                }
                if (!killer.IsHost)
                {
                    killer.SendHint(string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[killer]), 4);
                }
            }
        }
    }
}
