using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using static Broadcast;
using Player = Exiled.API.Features.Player;

namespace UsefulHints
{
    public class MVP : Plugin
    {
        private Dictionary<Player, int> scpKills = new Dictionary<Player, int>();

        private Dictionary<Player, int> humanKills = new Dictionary<Player, int>();

        private Player firstEscaper = null;

        private Player firstDeath = null;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.Escaping += OnPlayerEscaping;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Escaping -= OnPlayerEscaping;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRestartingRound;
            base.OnDisabled();
        }
        // Handler for "When Player dies"
        private void OnPlayerDied(DiedEventArgs ev)
        {
            Player attacker = ev.Attacker;

            if (firstDeath == null)
            {
                firstDeath = ev.Player;
            }
            if (attacker != null && attacker != ev.Player)
            {
                if (attacker.Role.Team == Team.SCPs)
                {
                    if (!scpKills.ContainsKey(attacker))
                        scpKills[attacker] = 0;

                    scpKills[attacker]++;
                }
                else
                {
                    if (!humanKills.ContainsKey(attacker))
                        humanKills[attacker] = 0;

                    humanKills[attacker]++;
                }
            }
        }
        // Handler for Escaped Player
        private void OnPlayerEscaping(EscapingEventArgs ev)
        {
            if (firstEscaper == null)
            {
                firstEscaper = ev.Player;
            }
        }
        // Handler for End Round messages
        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            string text = "";

            Player humanKiller = GetTopKiller(humanKills);
            Player scpKiller = GetTopKiller(scpKills);

            if (humanKiller != null)
                text += string.Format(Config.HumanKillMessage, humanKiller.Nickname, humanKills[humanKiller]) + "\n";
            if (scpKiller != null)
                text += string.Format(Config.ScpKillMessage, scpKiller.Nickname, scpKills[scpKiller]) + "\n";
            if (firstEscaper != null)
                text += string.Format(Config.EscaperMessage, firstEscaper.Nickname) + "\n";
            if (firstDeath != null)
                text += string.Format(Config.FirstDeathMessage, firstDeath.Nickname) + "\n";
            if (!string.IsNullOrEmpty(text))
                Map.Broadcast(10, text, BroadcastFlags.Normal, true);
        }
        // Reset Handlers
        private void OnRestartingRound()
        {
            scpKills.Clear();
            humanKills.Clear();
            firstEscaper = null;
            firstDeath = null;
        }
        // Helper to find player with most kills
        private Player GetTopKiller(Dictionary<Player, int> kills)
        {
            Player topKiller = null;
            int maxKills = 0;

            foreach (var entry in kills)
            {
                if (entry.Value > maxKills)
                {
                    topKiller = entry.Key;
                    maxKills = entry.Value;
                }
            }
            return topKiller;
        }
    }
}