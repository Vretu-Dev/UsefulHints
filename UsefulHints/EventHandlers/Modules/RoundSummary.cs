using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using static Broadcast;

namespace UsefulHints.EventHandlers.Modules
{
    public static class RoundSummary
    {
        private static Config Config => UsefulHints.Instance.Config;
        private static Dictionary<Player, int> scpKills = new Dictionary<Player, int>();
        private static Dictionary<Player, int> humanKills = new Dictionary<Player, int>();
        private static Dictionary<Player, float> humanDamage = new Dictionary<Player, float>();

        private static Player firstEscaper = null;
        private static Player firstScpKiller = null;
        private static DateTime roundStartTime;
        private static TimeSpan escapeTime;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
            Exiled.Events.Handlers.Player.Escaping += OnPlayerEscaping;
            Exiled.Events.Handlers.Player.Hurting += OnPlayerHurting;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
            Exiled.Events.Handlers.Player.Escaping -= OnPlayerEscaping;
            Exiled.Events.Handlers.Player.Hurting -= OnPlayerHurting;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        }

        private static void OnRoundStarted()
        {
            roundStartTime = DateTime.Now;
        }

        // Handler for player hurting another player
        private static void OnPlayerHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker == ev.Player || ev.Attacker.Role == null || ev.Player.Role == null)
                return;

            if (ev.Attacker.Role.Team != Team.SCPs && ev.Player.Role.Team == Team.SCPs)
            {
                if (!humanDamage.ContainsKey(ev.Attacker))
                    humanDamage[ev.Attacker] = 0f;

                humanDamage[ev.Attacker] += ev.Amount;
            }
        }

        // Handler for "When Player dying" (FirstScpKiller)
        private static void OnPlayerDying(DyingEventArgs ev)
        {
            if (ev.Player == null || ev.Player.Role == null)
                return;

            if (ev.Player.Role.Team == Team.SCPs && ev.Player.Role.Type != RoleTypeId.Scp0492)
            {
                if (firstScpKiller == null)
                {
                    firstScpKiller = ev.Attacker;
                }
            }
        }

        // Handler for "When Player died" (SCP and Human kill count)
        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker == ev.Player)
                return;

            if (ev.Attacker.Role.Team == Team.SCPs)
            {
                if (!scpKills.ContainsKey(ev.Attacker))
                    scpKills[ev.Attacker] = 0;

                scpKills[ev.Attacker]++;
            }
            else
            {
                if (!humanKills.ContainsKey(ev.Attacker))
                    humanKills[ev.Attacker] = 0;

                humanKills[ev.Attacker]++;
            }
        }

        // Handler for Escaped Player
        private static void OnPlayerEscaping(EscapingEventArgs ev)
        {
            if (firstEscaper == null && ev.IsAllowed && ev.Player != null)
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
                text += string.Format(Config.HumanKillMessage, humanKiller.Nickname, humanKills[humanKiller]) + "\n";
            if (scpKiller != null)
                text += string.Format(Config.ScpKillMessage, scpKiller.Nickname, scpKills[scpKiller]) + "\n";
            if (topDamageDealer != null)
                text += string.Format(Config.TopDamageMessage, topDamageDealer.Nickname, (int)Math.Round(humanDamage[topDamageDealer])) + "\n";
            if (firstEscaper != null)
                text += string.Format(Config.EscaperMessage, firstEscaper.Nickname, escapeTime.Minutes.ToString("D2"), escapeTime.Seconds.ToString("D2")) + "\n";
            if (firstScpKiller != null)
                text += string.Format(Config.FirstScpKillerMessage, firstScpKiller.Nickname) + "\n";

            if (!string.IsNullOrEmpty(text))
                Map.Broadcast(Config.RoundSummaryMessageDuration, text, BroadcastFlags.Normal, true);
        }

        // Clear Dictionaries
        private static void OnWaitingForPlayers()
        {
            scpKills.Clear();
            humanKills.Clear();
            humanDamage.Clear();
            firstEscaper = null;
            firstScpKiller = null;
            escapeTime = TimeSpan.Zero;
        }

        // Helper to find player with most kills
        private static Player GetTopKiller(Dictionary<Player, int> kills)
        {
            Player topKiller = null;
            int maxKills = 0;

            foreach (var entry in kills)
            {
                if (entry.Key == null)
                    continue;

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
            float maxDamage = 0f;

            foreach (var entry in damage)
            {
                if (entry.Key == null)
                    continue;

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
