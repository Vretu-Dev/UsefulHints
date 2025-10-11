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

        private static readonly Tag Scp096 = new("UH.SCP096");
        private static readonly Tag Grenade = new("UH.Grenade");
        private static readonly Tag Scp207 = new("UH.SCP207");
        private static readonly Tag Anti207 = new("UH.Anti207");
        private static readonly Tag Timer1576 = new("UH.Timer1576");
        private static readonly Tag Timer268 = new("UH.Timer268");
        private static readonly Tag Timer2176 = new("UH.Timer2176");
        private static readonly Tag Jailbird = new("UH.Jailbird");
        private static readonly Tag Warn = new("UH.Warn");
        private static readonly Tag FF = new("UH.FF");
        private static readonly Tag FFR = new("UH.FF.R");
        private static readonly Tag Cuffed = new("UH.Cuffed");
        private static readonly Tag CuffedR = new("UH.Cuffed.R");
        private static readonly Tag Kills = new("UH.Kills");
        private static readonly Tag Mates = new("UH.Mates");

        public static void Register()
        {
            if (Core.EnableHints)
            {
                Exiled.Events.Handlers.Scp096.AddingTarget += OnScp096AddingTarget;
                Exiled.Events.Handlers.Player.Hurting += OnGrenadeHurting;
                Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
                Exiled.Events.Handlers.Player.ChangingItem += OnEquipSCP207;
                Exiled.Events.Handlers.Player.UsedItem += OnSCP1576Used;
                Exiled.Events.Handlers.Player.ChangedItem += OnSCP1576ChangedItem;
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

        public static void Unregister()
        {
            if (Core.EnableHints)
            {
                Exiled.Events.Handlers.Scp096.AddingTarget -= OnScp096AddingTarget;
                Exiled.Events.Handlers.Player.Hurting -= OnGrenadeHurting;
                Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpSCP207;
                Exiled.Events.Handlers.Player.ChangingItem -= OnEquipSCP207;
                Exiled.Events.Handlers.Player.UsedItem -= OnSCP1576Used;
                Exiled.Events.Handlers.Player.ChangedItem -= OnSCP1576ChangedItem;
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
        private static void Show(Player player, string content, float duration, Tag tag)
        {
            if (player is null || player.ReferenceHub is null || string.IsNullOrWhiteSpace(content))
                return;

            Remove(player, tag);

            var el = new BasicElement(350f, content)
            {
                VerticalAlign = VerticalAlign.Center
            };

            Timing.CallDelayed(0.1f, () => RueDisplay.Get(player.ReferenceHub).Show(tag, el, duration));
        }

        private static void Remove(Player player, Tag tag)
        {
            if (player is null || player.ReferenceHub is null || tag is null)
                return;

            RueDisplay.Get(player.ReferenceHub).Remove(tag);
        }

        private static void ShowTimer(Player player, float seconds, Func<int, string> builder, Tag tag)
        {
            if (player is null || player.ReferenceHub is null || seconds <= 0)
                return;

            float end = Time.timeSinceLevelLoad + seconds;

            var element = new DynamicElement(350f, hub =>
            {
                int left = Math.Max(0, (int)Math.Ceiling(end - Time.timeSinceLevelLoad));
                return builder(left);
            })
            {
                UpdateInterval = TimeSpan.FromSeconds(1),
                VerticalAlign = VerticalAlign.Center,
            };

            RueDisplay.Get(player.ReferenceHub).Show(tag, element, seconds + 0.3f);
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
            if (!Core.ShowHintOnEquipItem)
                return;

            if (ev.Item == null || ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
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
            if (ev.Item.Type != ItemType.SCP1576)
                return;

            if (!ServerSettings.ShouldShowTimers(ev.Player))
                return;

            ShowTimer(ev.Player, 30f, left => $"<color=#FFA500>{new string('\n', 10)}{string.Format(Core.Scp1576TimeLeftMessage, left)}</color>", Timer1576);
        }

        private static void OnSCP1576ChangedItem(ChangedItemEventArgs ev)
        {
            Remove(ev.Player, Timer1576);
        }

        // SCP-268
        private static void OnSCP268Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type != ItemType.SCP268)
                return;

            if (!ServerSettings.ShouldShowTimers(ev.Player))
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
            if (ev.Projectile.Base is Scp2176Projectile)
            {
                if (ev.Player == null || !ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                ShowTimer(ev.Player, 13f, left => $"<color=#1CAA21>{new string('\n', 10)}{string.Format(Core.Scp2176TimeLeftMessage, left)}</color>", Timer2176);
            }
        }

        // Jailbird
        private static void OnPickingUpJailbird(PickingUpItemEventArgs ev)
        {
            if (!ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Pickup is Exiled.API.Features.Pickups.JailbirdPickup jailbirdPickup)
            {
                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdPickup.TotalCharges;
                string color = remainingCharges > 1 ? "#00B7EB" : "#C73804";
                string text = $"<color={color}>{new string('\n', 10)}{string.Format(Core.JailbirdUseMessage, remainingCharges)}</color>";
                Show(ev.Player, text, 4f, Jailbird);
            }
        }

        private static void OnEquipJailbird(ChangingItemEventArgs ev)
        {
            if (!Core.ShowHintOnEquipItem)
                return;

            if (ev.Item?.Base is JailbirdItem jailbirdItem && ServerSettings.ShouldShowHints(ev.Player))
            {
                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdItem.TotalChargesPerformed;
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
                    Show(ev.Player, string.Format(Core.Scp207Warning), 4f, Warn);

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type != ItemType.AntiSCP207)
                    Show(ev.Player, string.Format(Core.AntiScp207Warning), 4f, Warn);

                if (ev.Player.IsEffectActive<Scp1853>() && ev.Pickup.Type != ItemType.SCP1853)
                    Show(ev.Player, string.Format(Core.Scp1853Warning), 4f, Warn);
            }
        }

        // Friendly fire
        private static void OnFriendlyFireHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker.Role == null || ev.Player.Role == null || ev.Attacker.Role.Team == Team.SCPs || ev.Player.Role.Team == Team.SCPs || ev.Attacker == ev.Player)
                return;

            if (!ServerSettings.ShouldShowFFWarning(ev.Player) || !ServerSettings.ShouldShowFFWarning(ev.Attacker))
                return;

            if (ev.Attacker.Role.Side == ev.Player.Role.Side)
            {
                Show(ev.Attacker, string.Format(Core.FriendlyFireWarning), 1f, FF);
                Show(ev.Player, string.Format(Core.DamageTakenWarning, ev.Attacker.Nickname), 2f, FFR);
            }

            if (Core.EnableCuffedWarning && ev.Player.IsCuffed)
            {
                Show(ev.Attacker, string.Format(Core.CuffedAttackerWarning), 2f, Cuffed);
                Show(ev.Player, string.Format(Core.CuffedPlayerWarning, ev.Attacker.Nickname), 2f, CuffedR);
            }
        }

        // Kill counter
        private static readonly Dictionary<Player, int> PlayerKills = new();

        private static void OnWaitingForPlayers()
        {
            PlayerKills.Clear();
        }

        private static void OnPlayerDied(DiedEventArgs ev)
        {
            if (Core.CountPocketKills && ev.DamageHandler.Type == DamageType.PocketDimension)
            {
                var scp106 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp106);

                if (scp106 != null)
                {
                    AddKill(scp106);

                    if (!ServerSettings.ShouldShowKillCount(scp106))
                        return;

                    Show(scp106, string.Format(Core.KillCountMessage, PlayerKills[scp106]), 4f, Kills);
                }

                return;
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player)
            {
                AddKill(ev.Attacker);

                if (!ServerSettings.ShouldShowKillCount(ev.Attacker) || ev.Attacker.IsHost || ev.Player.IsHost)
                    return;

                Show(ev.Attacker, string.Format(Core.KillCountMessage, PlayerKills[ev.Attacker]), 4f, Kills);
            }
        }

        private static void AddKill(Player player)
        {
            if (player == null)
                return;

            if (PlayerKills.TryGetValue(player, out var c))
                PlayerKills[player] = c + 1;
            else
                PlayerKills[player] = 1;
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