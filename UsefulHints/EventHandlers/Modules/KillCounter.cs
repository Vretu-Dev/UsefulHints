using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Player;

namespace UsefulHints.EventHandlers.Modules
{
    public static class KillCounter
    {
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
        }
        private static void OnPlayerDied(DiedEventArgs ev)
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
                    killer.ShowHint(string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[killer]), 4);
                }
            }
        }
    }
}
