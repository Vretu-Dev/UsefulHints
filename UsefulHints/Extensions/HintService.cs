using Exiled.API.Features;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using System;
using System.Collections.Generic;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;

namespace UsefulHints.Extensions
{
    public static class PlayerExtensions
    {
        public static PlayerDisplay Display(this Player player) => PlayerDisplay.Get(player);
    }

    public static class HintService
    {
        // Entity Hints
        public static Hint Scp096LookHint(string message)
        {
            return new Hint
            {
                Text = message,
                YCoordinate = 600,
                FontSize = 32,
            };
        }

        // Item Hints
        public static DynamicHint GrenadeDamageHint(float amount, string template)
        {
            return new DynamicHint
            {
                Text = string.Format(template, Math.Round(amount)),
                TargetY = 700,
                FontSize = 32,
            };
        }

        public static DynamicHint Scp207Hint(int intensity, string template)
        {
            return new DynamicHint
            {
                Text = $"<color=#A60C0E>{string.Format(template, intensity)}</color>",
                TargetY = 800,
                FontSize = 32,
            };
        }

        public static DynamicHint AntiScp207Hint(int intensity, string template)
        {
            return new DynamicHint
            {
                Text = $"<color=#C53892>{string.Format(template, intensity)}</color>",
                TargetY = 800,
                FontSize = 32,
            };
        }
        
        public static DynamicHint JailbirdHint(int remainingCharges, string template, bool danger = false)
        {
            var color = danger ? "#C73804" : "#00B7EB";
            return new DynamicHint
            {
                Text = $"<color={color}>{string.Format(template, remainingCharges)}</color>",
                TargetY = 800,
                FontSize = 32,
            };
        }

        public static DynamicHint Scp207WarningHint(string message)
        {
            return new DynamicHint
            {
                Text = message,
                TargetY = 700,
                FontSize = 32,
            };
        }

        public static DynamicHint AntiScp207WarningHint(string message)
        {
            return new DynamicHint
            {
                Text = message,
                TargetY = 700,
                FontSize = 32,
            };
        }

        public static DynamicHint Scp1853WarningHint(string message)
        {
            return new DynamicHint
            {
                Text = message,
                TargetY = 700,
                FontSize = 32,
            };
        }

        // FFWarning
        public static Hint FriendlyFireWarning(string message)
        {
            return new Hint
            {
                Text = message,
                YCoordinate = 700,
                FontSize = 30,
            };
        }

        public static Hint DamageTakenWarning(string message, string attackerName)
        {
            return new Hint
            {
                Text = string.Format(message, attackerName),
                YCoordinate = 700,
                FontSize = 30,
            };
        }

        public static Hint CuffedAttackerWarning(string message)
        {
            return new Hint
            {
                Text = message,
                YCoordinate = 700,
                FontSize = 30,
            };
        }

        public static Hint CuffedPlayerWarning(string message, string attackerName)
        {
            return new Hint
            {
                Text = string.Format(message, attackerName),
                YCoordinate = 700,
                FontSize = 30,
            };
        }

        // Kill Counter
        public static DynamicHint KillCountHint(int kills, string message)
        {
            return new DynamicHint
            {
                Text = string.Format(message, kills),
                TargetY = 600,
                FontSize = 32
            };
        }

        // Teammates
        public static Hint TeammatesHint(List<string> teammates, string message)
        {
            return new Hint
            {
                Text = string.Format(message, string.Join("\n", teammates)),
                FontSize = 28,
                YCoordinate = 600,
                Alignment = HintAlignment.Left
            };
        }

        public static Hint AloneHint(string message)
        {
            return new Hint
            {
                Text = message,
                FontSize = 30,
                YCoordinate = 600,
                Alignment = HintAlignment.Left
            };
        }
    }
}
