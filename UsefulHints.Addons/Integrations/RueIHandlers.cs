using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using UnityEngine;
using UsefulHints.Extensions;

namespace UsefulHintsAddons.Integrations.RueI
{
    internal static class RueIHandlers
    {
        private static UsefulHints.Config Core => UsefulHints.UsefulHints.Instance.Config;

        private static object Scp096;
        private static object Grenade;
        private static object Scp207;
        private static object Anti207;
        private static object Timer1576;
        private static object Timer268;
        private static object Timer2176;
        private static object Jailbird;
        private static object Warn;
        private static object FF;
        private static object FFR;
        private static object Cuffed;
        private static object CuffedR;
        private static object Kills;
        private static object Mates;

        public static void Register()
        {
            InitializeTags();

            if (Core.EnableHints)
            {
                Exiled.Events.Handlers.Scp096.AddingTarget += OnScp096AddingTarget;
                Exiled.Events.Handlers.Player.Hurting += OnGrenadeHurting;
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
                Exiled.Events.Handlers.Player.ChangingItem += OnEquipSCP207;
                Exiled.Events.Handlers.Player.UsedItem += OnSCP1576Used;
                Exiled.Events.Handlers.Player.Scp1576TransmissionEnded += OnSCP1576TransmissionEnded;
                Exiled.Events.Handlers.Player.UsedItem += OnSCP268Used;
                Exiled.Events.Handlers.Player.InteractingDoor += OnSCP268Interacting;
                Exiled.Events.Handlers.Player.ChangedItem += OnSCP268ChangedItem;
                Exiled.Events.Handlers.Map.ExplodingGrenade += OnSCP2176Grenade;
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpJailbird;
                Exiled.Events.Handlers.Player.ChangingItem += OnEquipJailbird;
            }

            if (Core.EnableWarnings)
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpWarning;

            if (Core.EnableFfWarning)
                Exiled.Events.Handlers.Player.Hurting += OnFriendlyFireHurting;

            if (Core.EnableKillCounter)
            {
                Exiled.Events.Handlers.Player.Died += OnPlayerDied;
                Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            }

            if (Core.EnableTeammates)
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStartedTeammates;
        }

        private static void InitializeTags()
        {
            Scp096 = new Tag("UH.SCP096");
            Grenade = new Tag("UH.Grenade");
            Scp207 = new Tag("UH.SCP207");
            Anti207 = new Tag("UH.Anti207");
            Timer1576 = new Tag("UH.Timer1576");
            Timer268 = new Tag("UH.Timer268");
            Timer2176 = new Tag("UH.Timer2176");
            Jailbird = new Tag("UH.Jailbird");
            Warn = new Tag("UH.Warn");
            FF = new Tag("UH.FF");
            FFR = new Tag("UH.FF.R");
            Cuffed = new Tag("UH.Cuffed");
            CuffedR = new Tag("UH.Cuffed.R");
            Kills = new Tag("UH.Kills");
            Mates = new Tag("UH.Mates");
        }

        public static void Unregister()
        {
            if (Core.EnableHints)
            {
                Exiled.Events.Handlers.Scp096.AddingTarget -= OnScp096AddingTarget;
                Exiled.Events.Handlers.Player.Hurting -= OnGrenadeHurting;
                Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpSCP207;
                Exiled.Events.Handlers.Player.ChangingItem -= OnEquipSCP207;
                Exiled.Events.Handlers.Player.UsedItem -= OnSCP1576Used;
                Exiled.Events.Handlers.Player.Scp1576TransmissionEnded -= OnSCP1576TransmissionEnded;
                Exiled.Events.Handlers.Player.UsedItem -= OnSCP268Used;
                Exiled.Events.Handlers.Player.InteractingDoor -= OnSCP268Interacting;
                Exiled.Events.Handlers.Player.ChangedItem -= OnSCP268ChangedItem;
                Exiled.Events.Handlers.Map.ExplodingGrenade -= OnSCP2176Grenade;
                Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpJailbird;
                Exiled.Events.Handlers.Player.ChangingItem -= OnEquipJailbird;
            }

            if (Core.EnableWarnings)
                Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpWarning;

            if (Core.EnableFfWarning)
                Exiled.Events.Handlers.Player.Hurting -= OnFriendlyFireHurting;

            if (Core.EnableKillCounter)
            {
                Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
                Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            }

            if (Core.EnableTeammates)
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStartedTeammates;
        }

        // Helpers
        private static void Show(Player player, string content, float duration, object tag)
        {
            if (player == null || string.IsNullOrWhiteSpace(content) || tag == null)
                return;

            Remove(player, tag);

            // IMPORTANT: Object here to prevent the compiler from creating a field of type BasicElement 
            // in the auto-generated closure class, which would trigger ReflectionTypeLoadException if RueI is missing.
            object el = new BasicElement(350f, content)
            {
                VerticalAlign = VerticalAlign.Center
            };

            Timing.CallDelayed(0.1f, () => RueDisplay.Get(player).Show((Tag)tag, (BasicElement)el, duration));
        }

        private static void Remove(Player player, object tag)
        {
            if (player == null || tag == null)
                return;

            RueDisplay.Get(player).Remove((Tag)tag);
        }

        private static void ShowTimer(Player player, float seconds, Func<int, string> builder, object tag)
        {
            if (player == null || seconds <= 0 || tag == null)
                return;

            float end = Time.timeSinceLevelLoad + seconds;

            object element = new DynamicElement(350f, hub =>
            {
                int left = Math.Max(0, (int)Math.Ceiling(end - Time.timeSinceLevelLoad));
                return builder(left);
            })
            {
                UpdateInterval = TimeSpan.FromSeconds(1),
                VerticalAlign = VerticalAlign.Center,
            };

            RueDisplay.Get(player).Show((Tag)tag, (DynamicElement)element, seconds + 0.3f);
        }

        // SCP-096
        private static void OnScp096AddingTarget(Exiled.Events.EventArgs.Scp096.AddingTargetEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player == null || ev.Target == null || !ServerSettings.ShouldShowHints(ev.Target))
                return;

            Show(ev.Target, Core.Scp096LookMessage, 5f, Scp096);
        }

        // Grenade damage (attacker)
        private static void OnGrenadeHurting(HurtingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Amount <= 0.01f || ev.Attacker == null || ev.Player == null || ev.Player == ev.Attacker || !ServerSettings.ShouldShowHints(ev.Attacker))
                return;

            if (ev.DamageHandler.Type == DamageType.Explosion)
            {
                float remaining = ev.Player.Health - ev.Amount;
                if (remaining > 0 && !ev.Attacker.IsHost)
                {
                    string text = $"<color=white>{new string('\n', 5)}{string.Format(Core.GrenadeDamageHint, Math.Round(ev.Amount))}</color>";
                    Show(ev.Attacker, text, 4f, Grenade);
                }
            }
        }

        // SCP-207 pickup/equip
        private static void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type == ItemType.SCP207)
            {
                string text = $"<color=#A60C0E>{new string('\n', 10)}{string.Format(Core.Scp207HintMessage, ev.Player.GetEffect(EffectType.Scp207).Intensity)}</color>";
                Show(ev.Player, text, 4f, Scp207);
            }

            if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type == ItemType.AntiSCP207)
            {
                string text = $"<color=#C53892>{new string('\n', 10)}{string.Format(Core.AntiScp207HintMessage, ev.Player.GetEffect(EffectType.AntiScp207).Intensity)}</color>";
                Show(ev.Player, text, 4f, Anti207);
            }
        }

        private static void OnEquipSCP207(ChangingItemEventArgs ev)
        {
            if (!Core.ShowHintOnEquipItem || ev.Item == null || ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Player.IsEffectActive<Scp207>() && ev.Item.Type == ItemType.SCP207)
            {
                string text = $"<color=#A60C0E>{new string('\n', 10)}{string.Format(Core.Scp207HintMessage, ev.Player.GetEffect(EffectType.Scp207).Intensity)}</color>";
                Show(ev.Player, text, 4f, Scp207);
            }

            if (ev.Player.IsEffectActive<AntiScp207>() && ev.Item.Type == ItemType.AntiSCP207)
            {
                string text = $"<color=#C53892>{new string('\n', 10)}{string.Format(Core.AntiScp207HintMessage, ev.Player.GetEffect(EffectType.AntiScp207).Intensity)}</color>";
                Show(ev.Player, text, 4f, Anti207);
            }
        }

        // SCP-1576
        private static void OnSCP1576Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type != ItemType.SCP1576 || !ServerSettings.ShouldShowTimers(ev.Player))
                return;

            ShowTimer(ev.Player, 30f, left => $"<color=#FFA500>{new string('\n', 10)}{string.Format(Core.Scp1576TimeLeftMessage, left)}</color>", Timer1576);
        }

        private static void OnSCP1576TransmissionEnded(Scp1576TransmissionEndedEventArgs ev)
        {
            Remove(ev.Player, Timer1576);
        }

        // SCP-268
        private static void OnSCP268Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type != ItemType.SCP268 || !ServerSettings.ShouldShowTimers(ev.Player))
                return;

            ShowTimer(ev.Player, 15f, left => $"<color=purple>{new string('\n', 10)}{string.Format(Core.Scp268TimeLeftMessage, left)}</color>", Timer268);
        }

        private static void OnSCP268Interacting(InteractingDoorEventArgs ev)
        {
            Remove(ev.Player, Timer268);
        }

        private static void OnSCP268ChangedItem(ChangedItemEventArgs ev)
        {
            Remove(ev.Player, Timer268);
        }

        // SCP-2176
        private static void OnSCP2176Grenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.Base is Scp2176Projectile && ev.Player != null && ServerSettings.ShouldShowTimers(ev.Player))
            {
                ShowTimer(ev.Player, 13f, left => $"<color=#1CAA21>{new string('\n', 10)}{string.Format(Core.Scp2176TimeLeftMessage, left)}</color>", Timer2176);
            }
        }

        // Jailbird
        private static void OnPickingUpJailbird(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Pickup is Exiled.API.Features.Pickups.JailbirdPickup jailbirdPickup)
            {
                int remainingCharges = 5 - jailbirdPickup.TotalCharges;
                string color = remainingCharges > 1 ? "#00B7EB" : "#C73804";
                string text = $"<color={color}>{new string('\n', 10)}{string.Format(Core.JailbirdUseMessage, remainingCharges)}</color>";
                Show(ev.Player, text, 4f, Jailbird);
            }
        }

        private static void OnEquipJailbird(ChangingItemEventArgs ev)
        {
            if (!Core.ShowHintOnEquipItem || ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Item?.Base is JailbirdItem jailbirdItem)
            {
                int remainingCharges = 5 - jailbirdItem.TotalChargesPerformed;
                string color = remainingCharges > 1 ? "#00B7EB" : "#C73804";
                string text = $"<color={color}>{new string('\n', 10)}{string.Format(Core.JailbirdUseMessage, remainingCharges)}</color>";
                Show(ev.Player, text, 2f, Jailbird);
            }
        }

        // Warning hints
        private static void OnPickingUpWarning(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || !ServerSettings.ShouldShowWarningHints(ev.Player))
                return;

            if (ev.Pickup.Type == ItemType.SCP207 || ev.Pickup.Type == ItemType.AntiSCP207 || ev.Pickup.Type == ItemType.SCP1853)
            {
                if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type != ItemType.SCP207)
                    Show(ev.Player, Core.Scp207Warning, 4f, Warn);

                else if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                    Show(ev.Player, Core.AntiScp207Warning, 4f, Warn);

                else if (ev.Player.IsEffectActive<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                    Show(ev.Player, Core.Scp1853Warning, 4f, Warn);
            }
        }

        // Friendly fire
        private static void OnFriendlyFireHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker == ev.Player)
                return;

            if (ev.Attacker.Role.Team == Team.SCPs || ev.Player.Role.Team == Team.SCPs)
                return;

            if (!ServerSettings.ShouldShowFFWarning(ev.Player) || !ServerSettings.ShouldShowFFWarning(ev.Attacker))
                return;

            if (ev.Attacker.Role.Side == ev.Player.Role.Side)
            {
                Show(ev.Attacker, Core.FriendlyFireWarning, 1f, FF);
                Show(ev.Player, string.Format(Core.DamageTakenWarning, ev.Attacker.Nickname), 2f, FFR);
            }

            if (Core.EnableCuffedWarning && ev.Player.IsCuffed)
            {
                Show(ev.Attacker, Core.CuffedAttackerWarning, 2f, Cuffed);
                Show(ev.Player, string.Format(Core.CuffedPlayerWarning, ev.Attacker.Nickname), 2f, CuffedR);
            }
        }

        // Kill counter
        private static readonly Dictionary<Player, int> PlayerKills = new();

        private static void OnWaitingForPlayers() => PlayerKills.Clear();

        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (Core.CountPocketKills && ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                var scp106 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp106);
                if (scp106 != null)
                {
                    IncrementKills(scp106);
                    if (ServerSettings.ShouldShowKillCount(scp106))
                        Show(scp106, string.Format(Core.KillCountMessage, PlayerKills[scp106]), 4f, Kills);
                }
                return;
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                IncrementKills(ev.Attacker);
                if (!ev.Attacker.IsHost && !ev.Player.IsHost && ServerSettings.ShouldShowKillCount(ev.Attacker))
                    Show(ev.Attacker, string.Format(Core.KillCountMessage, PlayerKills[ev.Attacker]), 4f, Kills);
            }
        }

        private static void IncrementKills(Player player)
        {
            if (player == null)
                return;

            PlayerKills[player] = PlayerKills.TryGetValue(player, out var c) ? c + 1 : 1;
        }

        // Teammates
        private static void OnRoundStartedTeammates()
        {
            if (!Core.EnableTeammates)
                return;

            foreach (var player in Player.List)
            {
                var team = player.Role.Team;
                var mates = Player.List.Where(p => p.Role.Team == team && p != player).Select(p => p.Nickname).ToList();

                if (mates.Count > 0)
                {
                    Show(player, string.Format(Core.TeammateHintMessage, string.Join("\n", mates)), Core.TeammateMessageDuration, Mates);
                }
                else
                {
                    Show(player, string.Format(Core.AloneHintMessage), Core.AloneMessageDuration, Mates);
                }
            }
        }
    }
}