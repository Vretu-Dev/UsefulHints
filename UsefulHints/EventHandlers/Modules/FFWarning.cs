using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UsefulHints.Extensions;

namespace UsefulHints.EventHandlers.Modules
{
    public static class FFWarning
    {
        private static Config Config => UsefulHints.Instance.Config;

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
            if (ev.Attacker == null || ev.Player == null || ev.Attacker.Role == null || ev.Player.Role == null || ev.Attacker.Role.Team == Team.SCPs || ev.Player.Role.Team == Team.SCPs || ev.Attacker == ev.Player)
                return;

            if (!ServerSettings.ShouldShowFFWarning(ev.Player) || !ServerSettings.ShouldShowFFWarning(ev.Attacker))
                return;

            if (ev.Attacker.Role.Side == ev.Player.Role.Side)
            {
                if (ev.Attacker.Role.Team == Team.ClassD && ev.Player.Role.Team == Team.ClassD && Config.ClassDAreTeammates)
                {
                    ev.Attacker.ShowHint(string.Format(Config.FriendlyFireWarning), 1);
                    ev.Player.ShowHint(string.Format(Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                }
                else
                {
                    ev.Attacker.ShowHint(string.Format(Config.FriendlyFireWarning), 1);
                    ev.Player.ShowHint(string.Format(Config.DamageTakenWarning, ev.Attacker.Nickname), 2);
                }
            }

            if (Config.EnableCuffedWarning && ev.Player.IsCuffed)
            {
                ev.Attacker.ShowHint(string.Format(Config.CuffedAttackerWarning), 2);
                ev.Player.ShowHint(string.Format(Config.CuffedPlayerWarning, ev.Attacker.Nickname), 2);
            }
        }
    }
}