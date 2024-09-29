using System.Linq;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using MEC;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Teammates
    {
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStartedTeammates;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStartedTeammates;
        }
        private static IEnumerator<float> DelayedDisplayTeammates()
        {
            yield return Timing.WaitForSeconds(UsefulHints.Instance.Config.TeammateHintDelay);
            DisplayTeammates();
        }
        private static void OnRoundStartedTeammates()
        {
            Timing.RunCoroutine(DelayedDisplayTeammates());
        }
        private static void DisplayTeammates()
        {
            if (UsefulHints.Instance.Config.EnableTeammates)
            {
                foreach (var player in Player.List)
                {
                    List<string> teammates = Player.List
                        .Where(p => p.Role.Team == player.Role.Team && p != player)
                        .Select(p => p.Nickname)
                        .ToList();

                    if (teammates.Count > 0)
                    {
                        player.ShowHint(string.Format(UsefulHints.Instance.Config.TeammateHintMessage, string.Join("\n", teammates)), UsefulHints.Instance.Config.TeammateMessageDuration);
                    }
                    else
                    {
                        player.ShowHint(string.Format(UsefulHints.Instance.Config.AloneHintMessage), UsefulHints.Instance.Config.AloneMessageDuration);
                    }
                }
            }
        }
    }
}
