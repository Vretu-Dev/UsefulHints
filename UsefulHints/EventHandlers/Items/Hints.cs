using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using JailbirdPickup = Exiled.API.Features.Pickups.JailbirdPickup;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Jailbird;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Enum;
using CustomPlayerEffects;
using UsefulHints.Extensions;
using UnityEngine;

namespace UsefulHints.EventHandlers.Items
{
    public static class Hints
    {
        private static Config Config => UsefulHints.Instance.Config;
        // Timer SCP-1576
        private static readonly Dictionary<Player, float> active1576Timers = new Dictionary<Player, float>();
        private static readonly Dictionary<Player, DynamicHint> active1576Hints = new Dictionary<Player, DynamicHint>();

        // Timer SCP-268
        private static readonly Dictionary<Player, float> active268Timers = new Dictionary<Player, float>();
        private static readonly Dictionary<Player, DynamicHint> active268Hints = new Dictionary<Player, DynamicHint>();

        // Timer SCP-2176
        private static readonly Dictionary<Player, float> active2176Timers = new Dictionary<Player, float>();
        private static readonly Dictionary<Player, DynamicHint> active2176Hints = new Dictionary<Player, DynamicHint>();

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnGrenadeHurting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.ChangingItem += OnEquipSCP207;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP1576or268Used;
            Exiled.Events.Handlers.Player.InteractingDoor += OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP1576or268ChangeItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnSCP2176Grenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpJailbird;
            Exiled.Events.Handlers.Player.ChangingItem += OnEquipJailbird;
        }

        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnGrenadeHurting;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.ChangingItem -= OnEquipSCP207;
            Exiled.Events.Handlers.Player.UsedItem -= OnSCP1576or268Used;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem -= OnSCP1576or268ChangeItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnSCP2176Grenade;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpJailbird;
            Exiled.Events.Handlers.Player.ChangingItem -= OnEquipJailbird;
        }

        // Explosion Damage Handler
        private static void OnGrenadeHurting(HurtingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Amount <= 0.01f || ev.Attacker == null || ev.Player == null || ev.Player == ev.Attacker || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.DamageHandler.Type == DamageType.Explosion)
            {
                float RemainingHealth = ev.Player.Health - ev.Amount;

                if (RemainingHealth > 0 && !ev.Attacker.IsHost)
                {
                    var hint = HintService.GrenadeDamageHint(ev.Amount, Config.GrenadeDamageHint);

                    ev.Attacker.Display().AddHint(hint);
                    ev.Attacker.Display().RemoveAfter(hint, 4f);
                }
            }
        }

        // SCP 207 Handler
        private static void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type == ItemType.SCP207)
            {
                var hint = HintService.Scp207Hint(ev.Player.GetEffect(EffectType.Scp207).Intensity, Config.Scp207HintMessage);

                ev.Player.Display().AddHint(hint);
                ev.Player.Display().RemoveAfter(hint, 4f);
            }
            if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type == ItemType.AntiSCP207)
            {
                var hint = HintService.AntiScp207Hint(ev.Player.GetEffect(EffectType.AntiScp207).Intensity, Config.AntiScp207HintMessage);

                ev.Player.Display().AddHint(hint);
                ev.Player.Display().RemoveAfter(hint, 4f);
            }
        }

        private static void OnEquipSCP207(ChangingItemEventArgs ev)
        {
            if (Config.ShowHintOnEquipItem)
            {
                if (ev.Item == null || ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                    return;

                if (ev.Player.IsEffectActive<Scp207>() && ev.Item.Type == ItemType.SCP207)
                {
                    var hint = HintService.Scp207Hint(ev.Player.GetEffect(EffectType.Scp207).Intensity, Config.Scp207HintMessage);

                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 2f);
                }

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Item.Type == ItemType.AntiSCP207)
                {
                    var hint = HintService.AntiScp207Hint(ev.Player.GetEffect(EffectType.AntiScp207).Intensity, Config.AntiScp207HintMessage);

                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 2f);
                }
            }
        }
        
        // Add Timers on Event
        private static void OnSCP1576or268Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP1576)
            {
                if (!ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                StartOrRefreshTimer(ev.Player, 30f, active1576Timers, active1576Hints, "<color=#FFA500>{0}</color>", Config.Scp1576TimeLeftMessage, 900, 30);
            }
                

            if (ev.Item.Type == ItemType.SCP268)
            {
                if (!ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                StartOrRefreshTimer(ev.Player, 15f, active268Timers, active268Hints, "<color=purple>{0}</color>", Config.Scp268TimeLeftMessage, 900, 30);
            }
        }

        public static void OnSCP2176Grenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.Base is Scp2176Projectile)
            {
                if (ev.Player == null || !ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                StartOrRefreshTimer(ev.Player, 13f, active2176Timers, active2176Hints, "<color=#1CAA21>{0}</color>", Config.Scp2176TimeLeftMessage, 900, 30);
            }
        }

        // Remove Timers on Event
        private static void OnSCP1576or268ChangeItem(ChangedItemEventArgs ev)
        {
            StopTimer(ev.Player, active1576Timers, active1576Hints);
            StopTimer(ev.Player, active268Timers, active268Hints);
        }

        public static void OnSCP268Interacting(InteractingDoorEventArgs ev)
        {
            StopTimer(ev.Player, active268Timers, active268Hints);
        }

        // Template for Starting or Refreshing Timers
        private static void StartOrRefreshTimer(Player player, float duration, Dictionary<Player, float> timerDict, Dictionary<Player, DynamicHint> hintDict, string colorFormat, string msgFormat, int y, int fontSize)
        {
            float endTime = Time.time + duration;
            timerDict[player] = endTime;


            if (hintDict.TryGetValue(player, out var oldHint))
            {
                player.GetPlayerDisplay().RemoveHint(oldHint);
                hintDict.Remove(player);
            }

            var hint = new DynamicHint
            {
                AutoText = arg =>
                {
                    float tLeft = 0f;
                    if (timerDict.TryGetValue(player, out var hintEndTime))
                    {
                        tLeft = endTime - Time.time;
                        if (tLeft < 0f)
                            tLeft = 0f;
                    }

                    if (tLeft <= 0f)
                        return string.Empty;

                    return string.Format(colorFormat, string.Format(msgFormat, (int)tLeft));
                },
                FontSize = fontSize,
                TargetY = y,
                SyncSpeed = HintSyncSpeed.Fast
            };

            hintDict[player] = hint;
            player.GetPlayerDisplay().AddHint(hint);
        }

        private static void StopTimer(Player player, Dictionary<Player, float> timerDict, Dictionary<Player, DynamicHint> hintDict)
        {
            timerDict.Remove(player);

            if (hintDict.TryGetValue(player, out var hint))
            {
                player.GetPlayerDisplay().RemoveHint(hint);
                hintDict.Remove(player);
            }
        }

        // Reset Hints
        private static void OnWaitingForPlayers()
        {
            foreach (var kvp in active1576Hints)
            {
                kvp.Key.GetPlayerDisplay().RemoveHint(kvp.Value);
            }
            active1576Hints.Clear();

            foreach (var kvp in active268Hints)
            {
                kvp.Key.GetPlayerDisplay().RemoveHint(kvp.Value);
            }
            active268Hints.Clear();

            foreach (var kvp in active2176Hints)
            {
                kvp.Key.GetPlayerDisplay().RemoveHint(kvp.Value);
            }
            active2176Hints.Clear();
        }

        // Jailbird Handler
        private static void OnPickingUpJailbird(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is JailbirdPickup jailbirdPickup)
            {
                if (!ServerSettings.ShouldShowHints(ev.Player))
                    return;

                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdPickup.TotalCharges;

                var hint = HintService.JailbirdHint(remainingCharges, Config.JailbirdUseMessage, remainingCharges <= 1);

                ev.Player.Display().AddHint(hint);
                ev.Player.Display().RemoveAfter(hint, 2f);
            }
        }

        private static void OnEquipJailbird(ChangingItemEventArgs ev)
        {
            if (Config.ShowHintOnEquipItem)
            {
                if (ev.Item == null)
                    return;

                if (ev.Item.Base is JailbirdItem jailbirdItem)
                {
                    if (!ServerSettings.ShouldShowHints(ev.Player))
                        return;

                    int maxCharges = 5;
                    int remainingCharges = maxCharges - jailbirdItem.TotalChargesPerformed;

                    var hint = HintService.JailbirdHint(remainingCharges, Config.JailbirdUseMessage, remainingCharges <= 1);

                    ev.Player.Display().AddHint(hint);
                    ev.Player.Display().RemoveAfter(hint, 2f);
                }
            }
        }
    }
}