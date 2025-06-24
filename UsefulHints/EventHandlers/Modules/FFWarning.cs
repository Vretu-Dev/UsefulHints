using LabApi.Events.Arguments.PlayerEvents;
using PlayerRoles;

namespace UsefulHints.EventHandlers.Modules
{
    public static class FFWarning
    {
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Hurting += OnHurting;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Hurting -= OnHurting;
        }
        private static void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker.Team == Team.SCPs || ev.Player.Team == Team.SCPs || ev.Attacker == ev.Player || ev.Player.Nickname == null || ev.Attacker.Nickname == null || !ev.Player.IsAlive || !ev.Attacker.IsAlive)
                return;

            if (ev.Attacker.Role.GetFaction() == ev.Player.Role.GetFaction())
            {
                if (ev.Attacker.Team == Team.ClassD && ev.Player.Team == Team.ClassD && UsefulHints.Instance.Config.ClassDAreTeammates)
                {
                    ev.Attacker.SendHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                }
                else
                {
                    ev.Attacker.SendHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                }
            }
            if (UsefulHints.Instance.Config.EnableCuffedWarning && ev.Player.IsDisarmed)
            {
                ev.Attacker.SendHint(string.Format(UsefulHints.Instance.Config.CuffedAttackerWarning), 2);
                ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.CuffedPlayerWarning, ev.Attacker.Nickname), 2);
            }
        }
    }
}