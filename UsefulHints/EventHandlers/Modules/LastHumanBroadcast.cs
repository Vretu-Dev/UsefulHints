using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles;

namespace UsefulHints.EventHandlers.Modules
{
    public static class LastHumanBroadcast
    {
        private static bool hasBroadcastBeenSent = false;
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
            if (ev.Player == null || ev.Attacker == null)
                return;

            var aliveHumans = Player.List.Where(p => p.IsAlive && p.IsHuman && (!UsefulHints.Instance.Config.IgnoreTutorialRole || p.Role != RoleTypeId.Tutorial));

            int count = aliveHumans.Count();

            if (count > 1)
                hasBroadcastBeenSent = false;

            if (count == 1 && !hasBroadcastBeenSent)
            {
                Player lastAlive = aliveHumans.First();

                lastAlive.SendBroadcast(UsefulHints.Instance.Config.BroadcastForHuman, 10);

                var zone = GetZoneName(lastAlive);
                var teamName = GetRoleTeamName(lastAlive);

                string message = string.Format(UsefulHints.Instance.Config.BroadcastForScp, lastAlive.Nickname, teamName, zone);

                foreach (var scp in Player.List.Where(p => p.Team == Team.SCPs))
                {
                    scp.SendBroadcast(message, 10);
                }

                hasBroadcastBeenSent = true;
            }
        }
        private static string GetZoneName(Player player)
        {
            FacilityZone zone = player.Zone;

            switch (zone)
            {
                case FacilityZone.LightContainment:
                    return "Light Containment";
                case FacilityZone.HeavyContainment:
                    return "Heavy Containment";
                case FacilityZone.Entrance:
                    return "Entrance Zone";
                case FacilityZone.Surface:
                    return "Surface";
                default:
                    return "Unknown Zone";
            }
        }
        private static string GetRoleTeamName(Player player)
        {
            Team team = player.Team;

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
