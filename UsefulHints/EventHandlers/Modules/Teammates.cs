using System.Linq;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using MEC;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Models.Hints;

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
            foreach (var player in Player.List)
            {
                List<string> teammates = Player.List
                    .Where(p => p.Role.Team == player.Role.Team && p != player)
                    .Select(p => p.Nickname)
                    .ToList();

                PlayerDisplay playerDisplay = PlayerDisplay.Get(player);

                var TeammatesHint = new Hint
                {
                    Text = string.Format(UsefulHints.Instance.Config.TeammateHintMessage, string.Join("\n", teammates)),
                };
                var AloneHint = new Hint
                {
                    Text = string.Format(UsefulHints.Instance.Config.AloneHintMessage),
                };

                if (teammates.Count > 0)
                {
                    playerDisplay.AddHint(TeammatesHint);
                    Timing.CallDelayed(UsefulHints.Instance.Config.TeammateMessageDuration, () => { playerDisplay.RemoveHint(TeammatesHint); });
                }
                else
                {
                    playerDisplay.AddHint(AloneHint);
                    Timing.CallDelayed(UsefulHints.Instance.Config.TeammateMessageDuration, () => { playerDisplay.RemoveHint(AloneHint); });
                }
            }
        }
    }
}
