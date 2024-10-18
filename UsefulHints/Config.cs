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
        [Description("Hints Settings:")]
        public bool EnableHints { get; set; } = true;
        public string Scp096LookMessage { get; set; } = "You looked at SCP-096!";
        public float Scp268Duration { get; set; } = 15f;
        public string Scp268TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string Scp2176TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string Scp1576TimeLeftMessage { get; set; } = "Remaining: {0}s";
        public string JailbirdUseMessage { get; set; } = "Remaining charges: {0}";
        public string MicroEnergyMessage { get; set; } = "Remaining energy: {0}%";
        public string MicroLowEnergyMessage { get; set; } = "Low Energy";
        [Description("Jailbird and MicroHID hints on equip (default: only on pickup)")]
        public bool ShowHintOnEquip { get; set; } = true;
        public string Scp207HintMessage { get; set; } = "You have {0} doses of SCP-207";
        public string AntiScp207HintMessage { get; set; } = "You have {0} doses of Anti SCP-207";
        [Description("Item Warnings:")]
        public bool EnableWarnings { get; set; } = true;
        public string Scp207Warning { get; set; } = "<color=yellow>\u26A0</color> You are already affected by <color=#A60C0E>SCP-207</color>";
        public string AntiScp207Warning { get; set; } = "<color=yellow>\u26A0</color> You are already affected by <color=#2969AD>Anti SCP-207</color>";
        public string Scp1853Warning { get; set; } = "<color=yellow>\u26A0</color> You are already affected by <color=#1CAA21>SCP-1853</color>";
        [Description("Friendly Fire Warning:")]
        public bool EnableFfWarning { get; set; } = true;
        public string FriendlyFireWarning { get; set; } = "<size=27><color=yellow>\u26A0 Do not hurt your teammate</color></size>";
        public string DamageTakenWarning { get; set; } = "<size=27><color=red>{0}</color> <color=yellow>(teammate) hit you</color></size>";
        public bool ClassDAreTeammates { get; set; } = true;
        public bool EnableCuffedWarning { get; set; } = true;
        public string CuffedAttackerWarning { get; set; } = "<size=27><color=yellow>\u26A0 Player is cuffed</color></size>";
        public string CuffedPlayerWarning { get; set; } = "<size=27><color=red>{0}</color> <color=yellow>hit you when you were cuffed</color></size>";
        [Description("Kill Counter:")]
        public bool EnableKillCounter { get; set; } = true;
        public string KillCountMessage { get; set; } = "{0} kills";
        [Description("Round Summary:")]
        public bool EnableRoundSummary { get; set; } = true;
        public ushort RoundSummaryMessageDuration { get; set; } = 10;
        public string HumanKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> had the most kills as a <color=green>Human</color>: <color=yellow>{1}</color></size>";
        public string ScpKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> had the most kills as a <color=red>SCP</color>: <color=yellow>{1}</color></size>";
        public string TopDamageMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> dealt the most damage: <color=yellow>{1}</color></size>";
        public string FirstScpKillerMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> was the first to kill an <color=red>SCP</color></size>";
        public string EscaperMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> escaped first from the facility: <color=yellow>{1}:{2}</color></size>";
        [Description("Teammates:")]
        public bool EnableTeammates { get; set; } = true;
        public float TeammateHintDelay { get; set; } = 4f;
        public string TeammateHintMessage { get; set; } = "<align=left><size=28><color=#70EE9C>Your Teammates</color></size> \n<size=25><color=yellow>{0}</color></size></align>";
        public float TeammateMessageDuration { get; set; } = 8f;
        public string AloneHintMessage { get; set; } = "<align=left><color=red>You are playing Solo</color></align>";
        public float AloneMessageDuration { get; set; } = 4f;
        [Description("Last Human Broadcast:")]
        public bool EnableLastHumanBroadcast { get; set; } = true;
        public string BroadcastForHuman { get; set; } = "<color=red>You are the last human alive!</color>";
        public string BroadcastForScp { get; set; } = "<color=#70EE9C>{0}</color> is the last human alive, playing as {1} in <color=yellow>{2}</color>";
        public bool IgnoreTutorialRole { get; set; } = true;

        [Description("Jailbird Custom Settings:")]
        public bool EnableCustomJailbirdSettings { get; set; } = false;
        public EffectType JailbirdEffect { get; set; } = EffectType.Flashed;
        public float JailbirdEffectDuration { get; set; } = 4f;
        public byte JailbirdEffectIntensity { get; set; } = 1;
    }
}