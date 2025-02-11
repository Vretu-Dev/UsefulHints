using System.Linq;
using System.Collections.Generic;
using Player = LabApi.Features.Wrappers.Player;
using MEC;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Teammates
    {
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStartedTeammates;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStartedTeammates;
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
            foreach (var player in Player.List)
            {
                List<string> teammates = Player.List
                    .Where(p => p.Role.Team == player.Role.Team && p != player)
                    .Select(p => p.Nickname)
                    .ToList();

                if (teammates.Count > 0)
                {
                    player.SendHint(string.Format(UsefulHints.Instance.Config.TeammateHintMessage, string.Join("\n", teammates)), UsefulHints.Instance.Config.TeammateMessageDuration);
                }
                else
                {
                    player.SendHint(string.Format(UsefulHints.Instance.Config.AloneHintMessage), UsefulHints.Instance.Config.AloneMessageDuration);
                }
            }
        }
    }
}
