using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using Exiled.API.Enums;
using PlayerRoles;

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
            if (ev.DamageHandler.Type == DamageType.PocketDimension && UsefulHints.Instance.Config.CountPocketKills)
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
                        if (!killer.IsHost)
                        {
                            var KillHint = new DynamicHint
                            {
                                Text = string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[killer]),
                                TargetY = 600,
                                FontSize = 32
                            };

                            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Attacker);
                            playerDisplay.AddHint(KillHint);
                            Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(KillHint); });
                        }
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
                if (!killer.IsHost)
                {
                    var KillHint = new DynamicHint
                    {
                        Text = string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[killer]),
                        TargetY = 600,
                        FontSize = 32
                    };

                    PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Attacker);
                    playerDisplay.AddHint(KillHint);
                    Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(KillHint); });
                }
            }
        }
    }
}
