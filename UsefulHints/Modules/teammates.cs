using Player = Exiled.API.Features.Player;
using System.Collections.Generic;
using System.Linq;
using MEC;

namespace UsefulHints
{
    public class Teammates : UsefulHints
    {
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStartedTeammates;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStartedTeammates;
            base.OnDisabled();
        }
        private IEnumerator<float> DelayedDisplayTeammates()
        {
            yield return Timing.WaitForSeconds(Config.TeammateHintDelay);
            DisplayTeammates();
        }
        private void OnRoundStartedTeammates()
        {
            Timing.RunCoroutine(DelayedDisplayTeammates());
        }
        private void DisplayTeammates()
        {
            if (Config.TeammateHintEnable)
            {
                foreach (var player in Player.List)
                {
                    List<string> teammates = Player.List
                        .Where(p => p.Role.Team == player.Role.Team && p != player)
                        .Select(p => p.Nickname)
                        .ToList();

                    if (teammates.Count > 0)
                    {
                        player.ShowHint(string.Format(Config.TeammateHintMessage, string.Join("\n", teammates)), Config.TeammateMessageDuration);
                    }
                    else
                    {
                        player.ShowHint(string.Format(Config.AloneHintMessage), Config.AloneMessageDuration);
                    }
                }
            }
        }
    }
}