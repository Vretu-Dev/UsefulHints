using System.Linq;
using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using UsefulHints.Extensions;
using HintServiceMeow.Core.Extension;
using MEC;

namespace UsefulHints.EventHandlers.Modules
{
    public static class Teammates
    {
        private static Config Config => UsefulHints.Instance.Config;

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
            yield return Timing.WaitForSeconds(Config.TeammateHintDelay);
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
                    var teammatesHint = HintService.TeammatesHint(teammates, Config.TeammateHintMessage);
                    player.Display().AddHint(teammatesHint);
                    player.Display().RemoveAfter(teammatesHint, Config.TeammateMessageDuration);
                }
                else
                {
                    var aloneHint = HintService.AloneHint(Config.AloneHintMessage);
                    player.Display().AddHint(aloneHint);
                    player.Display().RemoveAfter(aloneHint, Config.TeammateMessageDuration);
                }
            }
        }
    }
}
