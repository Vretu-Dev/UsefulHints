using System;
using System.Collections.Generic;
using Player = LabApi.Features.Wrappers.Player;
using PlayerRoles;
using static Broadcast;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;

namespace UsefulHints.EventHandlers.Modules
{
    public static class RoundSummary
    {
        private static Dictionary<Player, int> scpKills = new Dictionary<Player, int>();
        private static Dictionary<Player, int> humanKills = new Dictionary<Player, int>();
        private static Dictionary<Player, float> humanDamage = new Dictionary<Player, float>();

        private static Player firstEscaper = null;
        private static Player firstScpKiller = null;
        private static DateTime roundStartTime;
        private static TimeSpan escapeTime;

        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.Dying += OnPlayerDying;
            LabApi.Events.Handlers.PlayerEvents.Escaping += OnPlayerEscaping;
            LabApi.Events.Handlers.PlayerEvents.Hurting += OnPlayerHurting;
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted += OnRestartingRound;
            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStarted;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDied;
            LabApi.Events.Handlers.PlayerEvents.Dying -= OnPlayerDying;
            LabApi.Events.Handlers.PlayerEvents.Escaping -= OnPlayerEscaping;
            LabApi.Events.Handlers.PlayerEvents.Hurting -= OnPlayerHurting;
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted -= OnRestartingRound;
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStarted;
        }
        private static void OnRoundStarted()
        {
            roundStartTime = DateTime.Now;
        }
        // Handler for player hurting another player
        private static void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            Player attacker = ev.Player;
            Player victim = ev.Target;

            if (attacker != null && attacker != victim && attacker.Role.Team != Team.SCPs && victim.Role.Team == Team.SCPs)
            {
                if (!humanDamage.ContainsKey(attacker))
                    humanDamage[attacker] = 0;

                humanDamage[attacker] += (int)Math.Round(ev.Amount);
            }
        }
        // Handler for "When Player dying" (FirstScpKiller)
        private static void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            Player attacker = ev.Attacker;
            Player victim = ev.Player;

            if (victim.Role.Team == Team.SCPs && victim.Role.Type != RoleTypeId.Scp0492)
            {
                if (firstScpKiller == null)
                {
                    firstScpKiller = attacker;
                }
            }
        }
        // Handler for "When Player died" (SCP and Human kill count)
        private static void OnPlayerDied(PlayerDeathEventArgs ev)
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
        private static void OnPlayerEscaping(PlayerEscapingEventArgs ev)
        {
            if (firstEscaper == null && ev.IsAllowed)
            {
                firstEscaper = ev.Player;
                escapeTime = DateTime.Now - roundStartTime;
            }
        }
        // Handler for End Round messages
        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            string text = "";

            Player humanKiller = GetTopKiller(humanKills);
            Player scpKiller = GetTopKiller(scpKills);
            Player topDamageDealer = GetTopDamageDealer(humanDamage);

            if (humanKiller != null)
                text += string.Format(UsefulHints.Instance.Config.HumanKillMessage, humanKiller.Nickname, humanKills[humanKiller]) + "\n";
            if (scpKiller != null)
                text += string.Format(UsefulHints.Instance.Config.ScpKillMessage, scpKiller.Nickname, scpKills[scpKiller]) + "\n";
            if (topDamageDealer != null)
                text += string.Format(UsefulHints.Instance.Config.TopDamageMessage, topDamageDealer.Nickname, humanDamage[topDamageDealer]) + "\n";
            if (firstEscaper != null)
                text += string.Format(UsefulHints.Instance.Config.EscaperMessage, firstEscaper.Nickname, escapeTime.Minutes.ToString("D2"), escapeTime.Seconds.ToString("D2")) + "\n";
            if (firstScpKiller != null)
                text += string.Format(UsefulHints.Instance.Config.FirstScpKillerMessage, firstScpKiller.Nickname) + "\n";
            if (!string.IsNullOrEmpty(text))
                Map.SendBroadcast(UsefulHints.Instance.Config.RoundSummaryMessageDuration, text, BroadcastFlags.Normal, true);
        }
        // Reset Handlers
        private static void OnRestartingRound()
        {
            scpKills.Clear();
            humanKills.Clear();
            humanDamage.Clear();
            firstEscaper = null;
            firstScpKiller = null;
        }
        // Helper to find player with most kills
        private static Player GetTopKiller(Dictionary<Player, int> kills)
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
        private static Player GetTopDamageDealer(Dictionary<Player, float> damage)
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
