using Exiled.API.Interfaces;

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
        public string HumanKillMessage { get; set; } = "<b><color=green>Player</color> {0}</b> had the most kills: <b><color=yellow>{1}</color></b>";
        public string ScpKillMessage { get; set; } = "<b><color=red>SCP</color> {0}</b> had the most kills: <b><color=yellow>{1}</color></b>";
        public string EscaperMessage { get; set; } = "<b><color=green>Player</color> {0}</b> escaped <color=yellow>first</color> from the facility</b>";
        public string KillCountMessage { get; set; } = "You have {0} kills";
    }
}
