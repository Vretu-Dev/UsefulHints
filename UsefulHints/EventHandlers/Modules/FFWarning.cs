using Exiled.Events.EventArgs.Player;
using UsefulHints.Extensions;
using HintServiceMeow.Core.Extension;
using PlayerRoles;

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
                var attackerHint = HintService.FriendlyFireWarning(Config.FriendlyFireWarning);
                var playerHint = HintService.DamageTakenWarning(Config.DamageTakenWarning, ev.Attacker.Nickname);

                if (ev.Attacker.Role.Team == Team.ClassD && ev.Player.Role.Team == Team.ClassD && Config.ClassDAreTeammates)
                {
                    ev.Player.Display().AddHint(playerHint);
                    ev.Player.Display().RemoveAfter(playerHint, 2f);

                    ev.Attacker.Display().AddHint(attackerHint);
                    ev.Attacker.Display().RemoveAfter(attackerHint, 2f);

                }
                else
                {
                    ev.Player.Display().AddHint(playerHint);
                    ev.Player.Display().RemoveAfter(playerHint, 2f);

                    ev.Attacker.Display().AddHint(attackerHint);
                    ev.Attacker.Display().RemoveAfter(attackerHint, 2f);
                }
            }
            if (Config.EnableCuffedWarning && ev.Player.IsCuffed)
            {
                var attackerHint = HintService.FriendlyFireWarning(Config.FriendlyFireWarning);
                var playerHint = HintService.DamageTakenWarning(Config.DamageTakenWarning, ev.Attacker.Nickname);

                ev.Player.Display().AddHint(playerHint);
                ev.Player.Display().RemoveAfter(playerHint, 2f);

                ev.Attacker.Display().AddHint(attackerHint);
                ev.Attacker.Display().RemoveAfter(attackerHint, 2f);
            }
        }
    }
}