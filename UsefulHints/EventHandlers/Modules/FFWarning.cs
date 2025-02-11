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
            if (ev.Player != null && ev.Target != null && ev.Player.RoleBase.Team != Team.SCPs && ev.Target.RoleBase.Team != Team.SCPs)
            {
                if (ev.Player.Team == ev.Target.Team && ev.Player != ev.Target)
                {
                    if (ev.Player.RoleBase.Team == Team.ClassD && ev.Target.RoleBase.Team == Team.ClassD)
                    {
                        if (UsefulHints.Instance.Config.ClassDAreTeammates)
                        {
                            ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                            ev.Target.SendHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Player.Nickname), 2);
                        }
                    }
                    else
                    {
                        ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                        ev.Target.SendHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Player.Nickname), 2);
                    }
                }
                if (UsefulHints.Instance.Config.EnableCuffedWarning && ev.Target.IsCuffed && ev.Player != ev.Target)
                {
                    ev.Player.SendHint(string.Format(UsefulHints.Instance.Config.CuffedAttackerWarning), 2);
                    ev.Target.SendHint(string.Format(UsefulHints.Instance.Config.CuffedPlayerWarning, ev.Player.Nickname), 2);
                }
            }
        }
    }
}