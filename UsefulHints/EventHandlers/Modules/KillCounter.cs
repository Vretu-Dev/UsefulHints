using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Enums;
using PlayerRoles;
using UsefulHints.Extensions;

namespace UsefulHints.EventHandlers.Modules
{
    public static class KillCounter
    {
        private static Config Config => UsefulHints.Instance.Config;
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
            // Kill Counter for 106 Pocket Dimension kills
            if (ev.DamageHandler.Type == DamageType.PocketDimension && Config.CountPocketKills)
            {
                foreach (var player in Player.List)
                {
                    if (player.Role.Type == RoleTypeId.Scp106)
                    {
                        Player killer = player;

                        if (playerKills.ContainsKey(killer))
                        {
                            playerKills[killer]++;
                        }
                        else
                        {
                            playerKills[killer] = 1;
                        }
                        if (!ServerSettings.ShouldShowKillCount(killer) || ev.Player.IsHost || ev.Attacker.IsHost)
                            return;

                        killer.ShowHint(string.Format(Config.KillCountMessage, playerKills[killer]), 4);
                    }
                }
            }

            // Kill Counter for every kills
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

                if (!ServerSettings.ShouldShowKillCount(killer) || ev.Player.IsHost || ev.Attacker.IsHost)
                    return;

                killer.ShowHint(string.Format(Config.KillCountMessage, playerKills[killer]), 4);
            }
        }
    }
}
