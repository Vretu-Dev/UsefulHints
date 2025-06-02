using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using MEC;

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
                PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);
                PlayerDisplay attackerDisplay = PlayerDisplay.Get(ev.Attacker);

                var AttackerHint = new DynamicHint
                {
                    Text = string.Format(UsefulHints.Instance.Config.FriendlyFireWarning),
                    TargetY = 700,
                    FontSize = 30,
                };
                var PlayerHint = new DynamicHint
                {
                    Text = string.Format(UsefulHints.Instance.Config.DamageTakenWarning, ev.Attacker.Nickname),
                    TargetY = 700,
                    FontSize = 30,
                };
                var CuffedAttackerHint = new DynamicHint
                {
                    Text = string.Format(UsefulHints.Instance.Config.CuffedAttackerWarning),
                    TargetY = 700,
                    FontSize = 30,
                };
                var CuffedPlayerHint = new DynamicHint
                {
                    Text = string.Format(UsefulHints.Instance.Config.CuffedPlayerWarning, ev.Attacker.Nickname),
                    TargetY = 700,
                    FontSize = 30,
                };

                if (ev.Attacker.Role.Side == ev.Player.Role.Side && ev.Attacker != ev.Player)
                {
                    if (ev.Attacker.Role.Team == Team.ClassD && ev.Player.Role.Team == Team.ClassD)
                    {
                        if (UsefulHints.Instance.Config.ClassDAreTeammates)
                        {
                            
                            attackerDisplay.AddHint(AttackerHint);
                            Timing.CallDelayed(2f, () => { attackerDisplay.RemoveHint(AttackerHint); });

                            playerDisplay.AddHint(PlayerHint);
                            Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(PlayerHint); });
                        }
                    }
                    else
                    {
                        attackerDisplay.AddHint(AttackerHint);
                        Timing.CallDelayed(2f, () => { attackerDisplay.RemoveHint(AttackerHint); });

                        playerDisplay.AddHint(PlayerHint);
                        Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(PlayerHint); });
                    }
                }
                if (UsefulHints.Instance.Config.EnableCuffedWarning && ev.Player.IsCuffed && ev.Attacker != ev.Player)
                {
                    attackerDisplay.AddHint(CuffedAttackerHint);
                    Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(CuffedAttackerHint); });

                    playerDisplay.AddHint(CuffedPlayerHint);
                    Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(CuffedPlayerHint); });
                }
            }
        }
    }
}