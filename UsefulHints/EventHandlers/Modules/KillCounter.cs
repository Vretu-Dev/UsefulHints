using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using Exiled.API.Enums;
using PlayerRoles;
using UsefulHints.Extensions;
using HintServiceMeow.Core.Extension;

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

                        var hint = HintService.KillCountHint(playerKills[killer], Config.KillCountMessage);
                        ev.Attacker.Display().AddHint(hint);
                        ev.Attacker.Display().RemoveAfter(hint, 2f);
                    }
                }
            }

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

                var hint = HintService.KillCountHint(playerKills[killer], Config.KillCountMessage);
                ev.Attacker.Display().AddHint(hint);
                ev.Attacker.Display().RemoveAfter(hint, 2f);
            }
        }
    }
}
