﻿using Exiled.API.Interfaces;

namespace UsefulHints
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string Scp096LookMessage { get; set; } = "You looked at SCP-096!";
        public float Scp268Duration { get; set; } = 15f;
        public string Scp268TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string Scp2176TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string JailbirdUseMessage { get; set; } = "Jailbird has been used {0} times";
        public string HumanKillMessage { get; set; } = "<size=30><b><color=green>Player</color> {0}</b> had the most kills: <color=yellow>{1}</color></size>";
        public string ScpKillMessage { get; set; } = "<size=30><b><color=red>SCP</color> {0}</b> had the most kills: <color=yellow>{1}</color></size>";
        public string EscaperMessage { get; set; } = "<size=30><b><color=green>Player</color> {0}</b> escaped <color=yellow>first</color> from the facility</size>";
        public string FirstDeathMessage { get; set; } = "<size=30><b><color=green>Player</color> {0}</b> died <color=yellow>first</color></size>";
        public string KillCountMessage { get; set; } = "You have {0} kills";
    }
}
