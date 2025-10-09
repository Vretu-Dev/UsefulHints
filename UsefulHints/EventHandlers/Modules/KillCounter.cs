using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;
using UsefulHints.Extensions;
using System.Linq;

namespace UsefulHints.EventHandlers.Modules
{
    public static class KillCounter
    {
        private static Config Config => UsefulHints.Instance.Config;
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        private static void OnWaitingForPlayers()
        {
            playerKills.Clear();
        }

        private static void OnPlayerDied(DiedEventArgs ev)
        {
            // Kill Counter for 106 Pocket Dimension kills
            if (Config.CountPocketKills && ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                var scp106 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp106);

                if (scp106 != null)
                {
                    AddKill(scp106);

                    if (!ServerSettings.ShouldShowKillCount(scp106))
                        return;

                    scp106.ShowHint(string.Format(Config.KillCountMessage, playerKills[scp106]), 4);
                }
                return;
            }

            // Kill Counter for every kills
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                AddKill(ev.Attacker);

                if (!ServerSettings.ShouldShowKillCount(ev.Attacker) || ev.Attacker.IsHost || ev.Player.IsHost)
                    return;

                ev.Attacker.ShowHint(string.Format(Config.KillCountMessage, playerKills[ev.Attacker]), 4);
            }
        }

        private static void AddKill(Player player)
        {
            if (player == null)
                return;

            if (playerKills.TryGetValue(player, out var count))
                playerKills[player] = count + 1;
            else
                playerKills[player] = 1;
        }
    }
}