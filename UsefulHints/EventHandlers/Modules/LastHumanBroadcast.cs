using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

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
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
        }

        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null)
                return;

            var aliveHumans = Player.List.Where(p => p.IsAlive && p.IsHuman && (!Config.IgnoreTutorialRole || p.Role.Type != RoleTypeId.Tutorial));

            int count = aliveHumans.Count();

            if (count > 1)
                hasBroadcastBeenSent = false;

            if (count == 1 && !hasBroadcastBeenSent)
            {
                Player lastAlive = aliveHumans.First();

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
            ZoneType zone = player.CurrentRoom.Zone;

            switch (zone)
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
            Team team = player.Role.Team;

            switch (team)
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
