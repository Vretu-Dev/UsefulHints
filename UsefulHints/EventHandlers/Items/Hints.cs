using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using JailbirdPickup = Exiled.API.Features.Pickups.JailbirdPickup;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Jailbird;
using UsefulHints.Extensions;
using CustomPlayerEffects;
using MEC;
using Events.Scp1576;
using Events.Scp268Item;

namespace UsefulHints.EventHandlers.Items
{
    public static class Hints
    {
        private static readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();
        private static Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        private static Config Config => UsefulHints.Instance.Config;

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnGrenadeHurting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.ChangingItem += OnEquipSCP207;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP1576Used;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP1576ChangedItem;
            Scp1576Events.StoppingTransmission += OnSCP1576StoppingTransmission;
            Scp268Events.UsedScp268 += OnSCP268Using;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP268Used;
            Exiled.Events.Handlers.Player.InteractingDoor += OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP268ChangedItem;
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
            Exiled.Events.Handlers.Player.UsedItem -= OnSCP1576Used;
            Exiled.Events.Handlers.Player.ChangedItem -= OnSCP1576ChangedItem;
            Scp1576Events.StoppingTransmission -= OnSCP1576StoppingTransmission;
            Scp268Events.UsedScp268 += OnSCP268Using;
            Exiled.Events.Handlers.Player.UsedItem -= OnSCP268Used;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnSCP268Interacting;
            Exiled.Events.Handlers.Player.ChangedItem -= OnSCP268ChangedItem;
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
                    ev.Attacker.ShowHint($"<color=white>{new string('\n', 5)}{string.Format(Config.GrenadeDamageHint, Math.Round(ev.Amount))}</color>", 4);
            }
        }

        // SCP 207 Handler
        private static void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                return;

            if (ev.Player.IsEffectActive<Scp207>() && ev.Pickup.Type == ItemType.SCP207)
                ev.Player.ShowHint($"<color=#A60C0E>{new string('\n', 10)}{string.Format(Config.Scp207HintMessage, ev.Player.GetEffect(EffectType.Scp207).Intensity)}</color>", 4);

            if (ev.Player.IsEffectActive<AntiScp207>() && ev.Pickup.Type == ItemType.AntiSCP207)
                ev.Player.ShowHint($"<color=#C53892>{new string('\n', 10)}{string.Format(Config.AntiScp207HintMessage, ev.Player.GetEffect(EffectType.AntiScp207).Intensity)}</color>", 4);
        }

        private static void OnEquipSCP207(ChangingItemEventArgs ev)
        {
            if (Config.ShowHintOnEquipItem)
            {
                if (ev.Item == null || ev.Player == null || !ServerSettings.ShouldShowHints(ev.Player))
                    return;

                if (ev.Player.IsEffectActive<Scp207>() && ev.Item.Type == ItemType.SCP207)
                    ev.Player.ShowHint($"<color=#A60C0E>{new string('\n', 10)}{string.Format(Config.Scp207HintMessage, ev.Player.GetEffect(EffectType.Scp207).Intensity)}</color>", 4);

                if (ev.Player.IsEffectActive<AntiScp207>() && ev.Item.Type == ItemType.AntiSCP207)
                    ev.Player.ShowHint($"<color=#C53892>{new string('\n', 10)}{string.Format(Config.AntiScp207HintMessage, ev.Player.GetEffect(EffectType.AntiScp207).Intensity)}</color>", 4);
            }
        }

        // SCP 1576 Handler
        private static void OnSCP1576Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP1576)
            {
                if (!ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }
                if (activeItems.ContainsKey(ev.Player))
                {
                    activeItems.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp1576Timer(ev.Player));
                activeCoroutines.Add(ev.Player, coroutine);
                activeItems.Add(ev.Player, ev.Item.Type);
            }
        }

        private static void OnSCP1576ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP1576)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }

        private static void OnSCP1576StoppingTransmission(StoppingTransmissionEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP1576)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }

        private static IEnumerator<float> Scp1576Timer(Player player)
        {
            float duration = 30f;

            while (duration > 0)
            {
                player.ShowHint($"<color=#FFA500>{new string('\n', 10)}{string.Format(Config.Scp1576TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }

        // SCP 268 Handler
        private static void OnSCP268Used(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP268)
            {
                if (!ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }
                if (activeItems.ContainsKey(ev.Player))
                {
                    activeItems.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp268Timer(ev.Player));
                activeCoroutines.Add(ev.Player, coroutine);
                activeItems.Add(ev.Player, ev.Item.Type);
            }
        }

        private static void OnSCP268Interacting(InteractingDoorEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }

        private static void OnSCP268ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }

        private static void OnSCP268Using(UsedScp268EventArgs ev)
        {
            ev.Cooldown = 5f;
            ev.InvisibilityTime = 5f;
            ev.Player.ShowHint($"You are now invisible for {ev.InvisibilityTime} seconds!", 1.15f);
        }

        private static IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = 15f;

            while (duration > 0)
            {
                player.ShowHint($"<color=purple>{new string('\n', 10)}{string.Format(Config.Scp268TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }

        // SCP 2176 Handler
        private static void OnSCP2176Grenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.Base is Scp2176Projectile)
            {
                if (ev.Player == null || !ServerSettings.ShouldShowTimers(ev.Player))
                    return;

                if (activeCoroutines.ContainsKey(ev.Player))
                {
                    Timing.KillCoroutines(activeCoroutines[ev.Player]);
                    activeCoroutines.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp2176Timer(ev.Player));
                activeCoroutines.Add(ev.Player, coroutine);
            }
        }
        private static IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;

            while (duration > 0)
            {
                player.ShowHint($"<color=#1CAA21>{new string('\n', 10)}{string.Format(Config.Scp2176TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // Reset Coroutines
        private static void OnWaitingForPlayers()
        {
            activeCoroutines.Clear();
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

                if (remainingCharges > 1)
                    ev.Player.ShowHint($"<color=#00B7EB>{new string('\n', 10)}{string.Format(Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
                else
                    ev.Player.ShowHint($"<color=#C73804>{new string('\n', 10)}{string.Format(Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
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

                    if (remainingCharges > 1)
                        ev.Player.ShowHint($"<color=#00B7EB>{new string('\n', 10)}{string.Format(Config.JailbirdUseMessage, remainingCharges)}</color>", 2);
                    else
                        ev.Player.ShowHint($"<color=#C73804>{new string('\n', 10)}{string.Format(Config.JailbirdUseMessage, remainingCharges)}</color>", 2);
                }
            }
        }
    }
}