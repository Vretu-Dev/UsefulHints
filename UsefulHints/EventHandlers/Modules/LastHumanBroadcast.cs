﻿using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace UsefulHints.EventHandlers.Modules
{
    public static class LastHumanBroadcast
    {
        private static bool hasBroadcastBeenSent = false;
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
            var aliveHumans = Player.List.Where(p => p.IsAlive && IsHuman(p));

            if (aliveHumans.Count() > 1)
            {
                hasBroadcastBeenSent = false;
            }
            if (aliveHumans.Count() == 1 && !hasBroadcastBeenSent)
            {
                Player lastAlive = aliveHumans.First();

                lastAlive.Broadcast(10, UsefulHints.Instance.Config.BroadcastForHuman);

                var zone = GetZoneName(lastAlive);
                var teamName = GetRoleTeamName(lastAlive);

                string message = string.Format(UsefulHints.Instance.Config.BroadcastForScp, lastAlive.Nickname, teamName, zone);

                foreach (var scp in Player.List.Where(p => p.Role.Team == Team.SCPs))
                {
                    scp.Broadcast(10, message);
                }
                hasBroadcastBeenSent = true;
            }
        }
        private static bool IsHuman(Player player)
        {
            return (player.Role.Team == Team.FoundationForces || player.Role.Team == Team.ClassD || player.Role.Team == Team.Scientists || player.Role.Team == Team.ChaosInsurgency) && (!UsefulHints.Instance.Config.IgnoreTutorialRole || player.Role.Type != RoleTypeId.Tutorial);
        }
        private static string GetZoneName(Player player)
        {
            ZoneType zone = player.CurrentRoom.Zone;

            switch (zone)
            {
                case ZoneType.LightContainment:
                    return "Light Containment";
                case ZoneType.HeavyContainment:
                    return "Heavy Containment";
                case ZoneType.Entrance:
                    return "Entrance Zone";
                case ZoneType.Surface:
                    return "Surface";
                default:
                    return "Unknown Zone";
            }
        }
        private static string GetRoleTeamName(Player player)
        {
            Team team = player.Role.Team;

            switch (team)
            {
                case Team.FoundationForces:
                    return "<color=#0096FF>Mobile Task Force</color>";
                case Team.ClassD:
                    return "<color=#FF8E00>Class D</color>";
                case Team.Scientists:
                    return "<color=#FFFF7C>Scientist</color>";
                case Team.ChaosInsurgency:
                    return "<color=#15853D>Chaos Insurgency</color>";
                default:
                    return "Unknown Team";
            }
        }
    }
}
