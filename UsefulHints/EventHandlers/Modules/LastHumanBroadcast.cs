using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
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
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.WaveRespawned += OnRespawnedTeam;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.WaveRespawned -= OnRespawnedTeam;
        }

        private static void OnWaitingForPlayers() => hasBroadcastBeenSent = false;
        private static void OnRespawnedTeam(WaveRespawnedEventArgs ev) => hasBroadcastBeenSent = false;

        private static void OnPlayerDied(PlayerDeathEventArgs ev)
        {
            if (ev.Player == null)
                return;

            var aliveHumans = Player.ReadyList
                .Where(p => p.IsHuman && (!UsefulHints.Instance.Config.IgnoreTutorialRole || p.Role != RoleTypeId.Tutorial))
                .ToList();

            if (aliveHumans.Count > 1)
            {
                hasBroadcastBeenSent = false;
                return;
            }

            if (aliveHumans.Count == 1 && !hasBroadcastBeenSent)
            {
                Player lastAlive = aliveHumans[0];

                lastAlive.SendBroadcast(UsefulHints.Instance.Config.BroadcastForHuman, 10);

                var zone = GetZoneName(lastAlive);
                var teamName = GetRoleTeamName(lastAlive);

                string message = string.Format(UsefulHints.Instance.Config.BroadcastForScp, lastAlive.Nickname, teamName, zone);

                foreach (var scp in Player.ReadyList.Where(p => p.Team == Team.SCPs))
                {
                    scp.SendBroadcast(message, 10);
                }

                hasBroadcastBeenSent = true;
            }
        }
        private static string GetZoneName(Player player)
        {
            switch (player.Zone)
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
            switch (player.Team)
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
