﻿using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHints
{
    public class Translations : ITranslation
    {
        [Description("UsefulHints settings server-specific translations \n Hints:")]
        public string ShowHints { get; set; } = "Show Hints";
        public string ShowHintsDescription { get; set; } = "Enable or disable hints display.";
        [Description("Timers:")]
        public string ShowTimers { get; set; } = "Show Timers";
        public string ShowTimersDescription { get; set; } = "SCP-268, SCP-2176 and SCP-1576.";
        [Description("Item Warning:")]
        public string ItemWarningHints { get; set; } = "Item Warning Hints";
        public string ItemWarningHintsDescription { get; set; } = "Prevent use SCP-207, Anti SCP-207 and SCP-1853 together.";
        [Description("Friendly Fire Warning:")]
        public string FriendlyFireWarning { get; set; } = "Friendly Fire Warning";
        public string FriendlyFireWarningDescription { get; set; } = "Warnings when you deal or receive damage to/from your teammates.";
        [Description("Kill Counter:")]
        public string KillCounter { get; set; } = "Kill Counter";
        public string KillCounterDescription { get; set; } = "Enable or disable kill count display.";
        [Description("Buttons ON/OFF:")]
        public string ButtonOn { get; set; } = "ON";
        public string ButtonOff { get; set; } = "OFF";
    }
}