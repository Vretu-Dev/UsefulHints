﻿using Exiled.Events.EventArgs.Player;
using PlayerRoles;

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
            if (ev.Attacker != null && ev.Player != null && ev.Attacker.Role != null && ev.Player.Role != null && ev.Attacker.Role.Team != Team.SCPs && ev.Player.Role.Team != Team.SCPs)
            {
                if (ev.Attacker.Role.Side == ev.Player.Role.Side && ev.Attacker != ev.Player)
                {
                    if (ev.Attacker.Role.Team == Team.ClassD && ev.Player.Role.Team == Team.ClassD)
                    {
                        if (UsefulHints.Instance.Config.ClassDAreTeammates)
                        {
                            if (ev.Player.SessionVariables.TryGetValue("showFFWarning", out var showFFWarning) && !(bool)showFFWarning || ev.Attacker.SessionVariables.TryGetValue("showFFWarning", out showFFWarning) && !(bool)showFFWarning)
                                return;

                            ev.Attacker.ShowHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                            ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                        }
                    }
                    else
                    {
                        if (ev.Player.SessionVariables.TryGetValue("showFFWarning", out var showFFWarning) && !(bool)showFFWarning || ev.Attacker.SessionVariables.TryGetValue("showFFWarning", out showFFWarning) && !(bool)showFFWarning)
                            return;

                        ev.Attacker.ShowHint(string.Format(UsefulHints.Instance.Config.FriendlyFireWarning), 1);
                        ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                    }
                }
                if (UsefulHints.Instance.Config.EnableCuffedWarning && ev.Player.IsCuffed && ev.Attacker != ev.Player)
                {
                    if (ev.Player.SessionVariables.TryGetValue("showFFWarning", out var showFFWarning) && !(bool)showFFWarning || ev.Attacker.SessionVariables.TryGetValue("showFFWarning", out showFFWarning) && !(bool)showFFWarning)
                        return;

                    ev.Attacker.ShowHint(string.Format(UsefulHints.Instance.Config.CuffedAttackerWarning), 2);
                    ev.Player.ShowHint(string.Format(UsefulHints.Instance.Config.CuffedPlayerWarning, ev.Attacker.Nickname), 2);
                }
            }
        }
    }
}