using System.Collections.Generic;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Core.UserSettings;

namespace UsefulHints.EventHandlers.Modules
{
    public static class KillCounter
    {
        private static readonly Dictionary<Player, int> playerKills = new Dictionary<Player, int>();
        public static TwoButtonsSetting ShowKillCounterSetting { get; private set; }
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;

            ShowKillCounterSetting = new TwoButtonsSetting(
            id: 773,
            label: "Kill Counter",
            firstOption: "ON",
            secondOption: "OFF",
            defaultIsSecond: false,
            hintDescription: "Enable or disable kill count display.",
            onChanged: (player, setting) =>
            {
                var showKillCount = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                player.SessionVariables["ShowKillCount"] = showKillCount;
            });
            SettingBase.Register(new[] { ShowKillCounterSetting });
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            SettingBase.Unregister(settings: new[] { ShowKillCounterSetting });
        }
        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                Player killer = ev.Attacker;

                if (playerKills.ContainsKey(killer))
                {
                    playerKills[killer]++;
                }
                else
                {
                    playerKills[killer] = 1;
                }
                if (!killer.IsHost)
                {
                    if (killer.SessionVariables.TryGetValue("ShowKillCount", out var showKillCount) && !(bool)showKillCount)
                        return;

                    killer.ShowHint(string.Format(UsefulHints.Instance.Config.KillCountMessage, playerKills[killer]), 4);
                }
            }
        }
    }
}
