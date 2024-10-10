using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHints
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("[Module] Hints:")]
        public bool EnableHints { get; set; } = true;
        public string Scp096LookMessage { get; set; } = "You looked at SCP-096!";
        public float Scp268Duration { get; set; } = 15f;
        public string Scp268TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string Scp2176TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string JailbirdUseMessage { get; set; } = "Remaining charge: {0}";
        public string Scp207HintMessage { get; set; } = "You are on {0} SCP-207";
        public string AntiScp207HintMessage { get; set; } = "You are on {0} Anti SCP-207";
        [Description("[Module] Items Warnings:")]
        public bool EnableWarnings { get; set; } = true;
        public string Scp207Warning { get; set; } = "<color=yellow>\u26A0</color> You are already on <color=#A60C0E>SCP-207</color>";
        public string AntiScp207Warning { get; set; } = "<color=yellow>\u26A0</color> You are already on <color=#2969AD>Anti SCP-207</color>";
        public string Scp1853Warning { get; set; } = "<color=yellow>\u26A0</color> You are already on <color=#1CAA21>SCP-1853</color>";
        [Description("[Module] Friendly Fire Warning:")]
        public bool EnableFfWarning { get; set; } = true;
        public string FriendlyFireWarning { get; set; } = "<size=27><color=yellow>\u26A0 Not hurt your teammate</color></size>";
        public string DamageTakenAlert { get; set; } = "<size=27><color=red>{0}</color> <color=yellow>teammate damaging you</color></size>";
        public bool ClassDAreTeammates { get; set; } = true;
        [Description("[Module] Kill Counter:")]
        public bool EnableKillCounter { get; set; } = true;
        public string KillCountMessage { get; set; } = "{0} kills";
        [Description("[Module] Round Summary:")]
        public bool EnableRoundSummary { get; set; } = true;
        public ushort RoundSummaryMessageDuration { get; set; } = 10;
        public string HumanKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> had the most kills as <color=green>Human</color>: <color=yellow>{1}</color></size>";
        public string ScpKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> had the most kills as <color=red>SCP</color>: <color=yellow>{1}</color></size>";
        public string TopDamageMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> did the most damage: <color=yellow>{1}</color></size>";
        public string FirstScpKillerMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> was the first to kill <color=red>SCP</color></size>";
        public string EscaperMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> escaped first from the facility: <color=yellow>{1}:{2}</color></size>";
        [Description("[Module] Teammates:")]
        public bool EnableTeammates { get; set; } = true;
        public float TeammateHintDelay { get; set; } = 4f;
        public string TeammateHintMessage { get; set; } = "<align=left><size=28><color=#70EE9C>Your Teammates</color></size> \n<size=25><color=yellow>{0}</color></size></align>";
        public float TeammateMessageDuration { get; set; } = 8f;
        public string AloneHintMessage { get; set; } = "<align=left><color=red>You are playing Solo</color></align>";
        public float AloneMessageDuration { get; set; } = 4f;
        [Description("[Module] Last Human Broadcast:")]
        public bool EnableLastHumanBroadcast { get; set; } = true;
        public string BroadcastForHuman { get; set; } = "<color=red>You are the last human alive!</color>";
        public string BroadcastForScp { get; set; } = "<color=#70EE9C>{0}</color> is the last human alive playing as {1} in <color=yellow>{2}</color>";
        [Description("[Module] Jailbird Custom Settings:")]
        public bool EnableCustomJailbirdSettings { get; set; } = false;
        public EffectType JailbirdEffect { get; set; } = EffectType.Flashed;
        public float JailbirdEffectDuration { get; set; } = 4f;
        public byte JailbirdEffectIntensity { get; set; } = 1;
    }
}