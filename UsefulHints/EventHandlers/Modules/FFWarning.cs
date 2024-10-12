using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;

namespace UsefulHints.EventHandlers.Modules
{
    public static class FFWarning
    {
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }
        private static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Player != null && ev.Attacker.Role != null && ev.Player.Role != null)
            {
                if (ev.Attacker.Role.Side == ev.Player.Role.Side && ev.Attacker != ev.Player)
                {
                    if (ev.Attacker.Role.Team == Team.ClassD && ev.Player.Role.Team == Team.ClassD)
                    {
                        if (UsefulHints.Instance.Config.ClassDAreTeammates)
                        {
                            ev.Attacker.ShowHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                            ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                        }
                    }
                    else
                    {
                        ev.Attacker.ShowHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                        ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                    }
                }
            }
        }

    }
}