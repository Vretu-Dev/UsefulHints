using System;
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
        private Dictionary<Player, float> humanDamage = new Dictionary<Player, float>();

        private Player firstEscaper = null;
        private Player firstScpKiller = null;
        private DateTime roundStartTime;
        private TimeSpan escapeTime;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
            Exiled.Events.Handlers.Player.Escaping += OnPlayerEscaping;
            Exiled.Events.Handlers.Player.Hurting += OnPlayerHurting;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
            Exiled.Events.Handlers.Player.Escaping -= OnPlayerEscaping;
            Exiled.Events.Handlers.Player.Hurting -= OnPlayerHurting;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRestartingRound;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            base.OnDisabled();
        }
        private void OnRoundStarted()
        {
            roundStartTime = DateTime.Now;
        }
        // Handler for player hurting another player
        private void OnPlayerHurting(HurtingEventArgs ev)
        {
            Player attacker = ev.Attacker;
            Player victim = ev.Player;

            if (attacker != null && attacker != victim && attacker.Role.Team != Team.SCPs && victim.Role.Team == Team.SCPs)
            {
                if (!humanDamage.ContainsKey(attacker))
                    humanDamage[attacker] = 0;

                humanDamage[attacker] += (int)Math.Round(ev.Amount);
            }
        }
        // Handler for "When Player died"
        private void OnPlayerDying(DyingEventArgs ev)
        {
            Player attacker = ev.Attacker;
            Player victim = ev.Player;

            if (victim.Role.Team == Team.SCPs) // Check if the victim is an SCP
            {
                if (firstScpKiller == null) // Only set the first SCP killer if it's not already set
                {
                    firstScpKiller = attacker;
                }
            }
        }
        private void OnPlayerDied(DiedEventArgs ev)
        {
            Player attacker = ev.Attacker;
            
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
                escapeTime = DateTime.Now - roundStartTime;
            }
        }
        // Handler for End Round messages
        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            string text = "";

            Player humanKiller = GetTopKiller(humanKills);
            Player scpKiller = GetTopKiller(scpKills);
            Player topDamageDealer = GetTopDamageDealer(humanDamage);

            if (humanKiller != null)
                text += string.Format(Config.HumanKillMessage, humanKiller.Nickname, humanKills[humanKiller]) + "\n";
            if (scpKiller != null)
                text += string.Format(Config.ScpKillMessage, scpKiller.Nickname, scpKills[scpKiller]) + "\n";
            if (topDamageDealer != null)
                text += string.Format(Config.TopDamageMessage, topDamageDealer.Nickname, humanDamage[topDamageDealer]) + "\n";
            if (firstEscaper != null)
                text += string.Format(Config.EscaperMessage, firstEscaper.Nickname, escapeTime.Minutes, escapeTime.Seconds) + "\n";
            if (firstScpKiller != null)
                text += string.Format(Config.FirstScpKillerMessage, firstScpKiller.Nickname) + "\n";
            if (!string.IsNullOrEmpty(text))
                Map.Broadcast(10, text, BroadcastFlags.Normal, true);
        }
        // Reset Handlers
        private void OnRestartingRound()
        {
            scpKills.Clear();
            humanKills.Clear();
            humanDamage.Clear();
            firstEscaper = null;
            firstScpKiller = null;
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
        // Helper to find player with most damage
        private Player GetTopDamageDealer(Dictionary<Player, float> damage)
        {
            Player topDealer = null;
            float maxDamage = 0;

            foreach (var entry in damage)
            {
                if (entry.Value > maxDamage)
                {
                    topDealer = entry.Key;
                    maxDamage = entry.Value;
                }
            }
            return topDealer;
        }
    }
}