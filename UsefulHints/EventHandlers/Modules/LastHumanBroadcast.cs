using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Linq;

namespace UsefulHints.EventHandlers.Modules
{
    public static class LastHumanBroadcast
    {
        private static Config Config => UsefulHints.Instance.Config;
        private static Translations Translation => UsefulHints.Instance.Translation;
        private static bool hasBroadcastBeenSent = false;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RespawnedTeam += OnRespawnedTeam;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RespawnedTeam -= OnRespawnedTeam;
        }

        private static void OnWaitingForPlayers() => hasBroadcastBeenSent = false;
        private static void OnRespawnedTeam(RespawnedTeamEventArgs ev) => hasBroadcastBeenSent = false;

        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Player == null)
                return;

            var aliveHumans = Player.List
                .Where(p => p.IsHuman && (!Config.IgnoreTutorialRole || p.Role.Type != RoleTypeId.Tutorial))
                .ToList();

            if (aliveHumans.Count > 1)
            {
                hasBroadcastBeenSent = false;
                return;
            }

            if (aliveHumans.Count ==  1 && !hasBroadcastBeenSent)
            {
                Player lastAlive = aliveHumans[0];

                lastAlive.Broadcast(10, Config.BroadcastForHuman);

                var zone = GetZoneName(lastAlive);
                var teamName = GetRoleTeamName(lastAlive);

                string message = string.Format(Config.BroadcastForScp, lastAlive.Nickname, teamName, zone);

                foreach (var scp in Player.List.Where(p => p.Role.Team == Team.SCPs))
                {
                    scp.Broadcast(10, message);
                }

                hasBroadcastBeenSent = true;
            }
        }

        private static string GetZoneName(Player player)
        {
            if (player.CurrentRoom == null)
                return "Unknown Zone";

            switch (player.CurrentRoom.Zone)
            {
                case ZoneType.LightContainment:
                    return Translation.Lcz;
                case ZoneType.HeavyContainment:
                    return Translation.Hcz;
                case ZoneType.Entrance:
                    return Translation.Entrance;
                case ZoneType.Surface:
                    return Translation.Surface;
                default:
                    return "Unknown Zone";
            }
        }

        private static string GetRoleTeamName(Player player)
        {
            switch (player.Role.Team)
            {
                case Team.FoundationForces:
                    return Translation.FoundationForces;
                case Team.ClassD:
                    return Translation.ClassD;
                case Team.Scientists:
                    return Translation.Scientists;
                case Team.ChaosInsurgency:
                    return Translation.ChaosInsurgency;
                default:
                    return "Unknown Team";
            }
        }
    }
}
