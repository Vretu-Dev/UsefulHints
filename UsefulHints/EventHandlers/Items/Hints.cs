using System;
using Player = LabApi.Features.Wrappers.Player;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Jailbird;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using System.Collections.Generic;
using CustomPlayerEffects;
using PlayerStatsSystem;
using MEC;

namespace UsefulHints.EventHandlers.Items
{
    public static class Hints
    {
        private static readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();
        private static Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        public static void RegisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Hurting += OnGrenadeHurting;
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem += OnPickingUpSCP207;
            LabApi.Events.Handlers.PlayerEvents.ChangingItem += OnEquipSCP207;
            LabApi.Events.Handlers.PlayerEvents.UsedItem += OnSCP1576Used;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem += OnSCP1576ChangedItem;
            LabApi.Events.Handlers.PlayerEvents.UsedItem += OnSCP268Used;
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor += OnSCP268Interacting;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem += OnSCP268ChangedItem;
            LabApi.Events.Handlers.ServerEvents.GrenadeExploding += OnSCP2176Grenade;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem += OnPickingUpJailbird;
            LabApi.Events.Handlers.PlayerEvents.ChangingItem += OnEquipJailbird;
        }
        public static void UnregisterEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Hurting -= OnGrenadeHurting;
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem -= OnPickingUpSCP207;
            LabApi.Events.Handlers.PlayerEvents.ChangingItem -= OnEquipSCP207;
            LabApi.Events.Handlers.PlayerEvents.UsedItem -= OnSCP1576Used;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem -= OnSCP1576ChangedItem;
            LabApi.Events.Handlers.PlayerEvents.UsedItem -= OnSCP268Used;
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor -= OnSCP268Interacting;
            LabApi.Events.Handlers.PlayerEvents.ChangedItem -= OnSCP268ChangedItem;
            LabApi.Events.Handlers.ServerEvents.GrenadeExploding -= OnSCP2176Grenade;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            LabApi.Events.Handlers.PlayerEvents.PickingUpItem -= OnPickingUpJailbird;
            LabApi.Events.Handlers.PlayerEvents.ChangingItem -= OnEquipJailbird;
        }
        // Explosion Damage Handler
        private static void OnGrenadeHurting(PlayerHurtingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Target == null || ev.Player == null || ev.Player == ev.Target)
                return;

            if (ev.DamageHandler is ExplosionDamageHandler explosionDamageHandler)
            {
                float amount = explosionDamageHandler.Damage;
                float RemainingHealth = ev.Player.Health - amount;

                if (RemainingHealth > 0)
                    ev.Player.SendHint($"<color=white>{new string('\n', 5)}{string.Format(UsefulHints.Instance.Config.GrenadeDamageHint, Math.Round(amount))}</color>", 4);
            }
        }
        // SCP 207 Handler
        private static void OnPickingUpSCP207(PlayerPickingUpItemEventArgs ev)
        {
            if (ev.Player.HasEffect<Scp207>() && ev.Pickup.Type == ItemType.SCP207)
                ev.Player.SendHint($"<color=#A60C0E>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp207HintMessage, ev.Player.GetEffect<Scp207>().Intensity)}</color>", 4);

            if (ev.Player.HasEffect<AntiScp207>() && ev.Pickup.Type == ItemType.AntiSCP207)
                ev.Player.SendHint($"<color=#C53892>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.AntiScp207HintMessage, ev.Player.GetEffect<AntiScp207>().Intensity)}</color>", 4);
        }
        private static void OnEquipSCP207(PlayerChangingItemEventArgs ev)
        {
            if (UsefulHints.Instance.Config.ShowHintOnEquipItem)
            {
                if (ev.NewItem == null)
                    return;

                if (ev.Player.HasEffect<Scp207>() && ev.NewItem.Type == ItemType.SCP207)
                    ev.Player.SendHint($"<color=#A60C0E>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp207HintMessage, ev.Player.GetEffect<Scp207>().Intensity)}</color>", 4);

                if (ev.Player.HasEffect<AntiScp207>() && ev.NewItem.Type == ItemType.AntiSCP207)
                    ev.Player.SendHint($"<color=#C53892>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.AntiScp207HintMessage, ev.Player.GetEffect<AntiScp207>().Intensity)}</color>", 4);
            }
        }
        // SCP 1576 Handler
        private static void OnSCP1576Used(PlayerUsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP1576)
            {
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
        private static void OnSCP1576ChangedItem(PlayerChangedItemEventArgs ev)
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
                player.SendHint($"<color=#FFA500>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp1576TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // SCP 268 Handler
        private static void OnSCP268Used(PlayerUsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP268)
            {
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
        private static void OnSCP268Interacting(PlayerInteractingDoorEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private static void OnSCP268ChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);
            }
        }
        private static IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = 15f;

            while (duration > 0)
            {
                player.SendHint($"<color=purple>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp268TimeLeftMessage, (int)duration)}</color>", 1.15f);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }
            activeCoroutines.Remove(player);
        }
        // SCP 2176 Handler
        private static void OnSCP2176Grenade(GrenadeExplodingEventArgs ev)
        {
            if (ev.Grenade is Scp2176Projectile)
            {
                if (ev.Player != null)
                {
                    if (activeCoroutines.ContainsKey(ev.Player))
                    {
                        Timing.KillCoroutines(activeCoroutines[ev.Player]);
                        activeCoroutines.Remove(ev.Player);
                    }

                    var coroutine = Timing.RunCoroutine(Scp2176Timer(ev.Player));
                    activeCoroutines.Add(ev.Player, coroutine);
                }
            }
        }
        private static IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;

            while (duration > 0)
            {
                player.SendHint($"<color=#1CAA21>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.Scp2176TimeLeftMessage, (int)duration)}</color>", 1.15f);
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
        private static void OnPickingUpJailbird(PlayerPickingUpItemEventArgs ev)
        {
            if (ev.Pickup is JailbirdPickup jailbirdPickup)
            {
                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdPickup.TotalCharges;

                if (remainingCharges > 1)
                    ev.Player.SendHint($"<color=#00B7EB>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
                else
                    ev.Player.SendHint($"<color=#C73804>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 4);
            }
        }
        private static void OnEquipJailbird(PlayerChangingItemEventArgs ev)
        {
            if (UsefulHints.Instance.Config.ShowHintOnEquipItem)
            {
                if (ev.NewItem == null)
                    return;

                if (ev.NewItem.Base is JailbirdItem jailbirdItem)
                {
                    int maxCharges = 5;
                    int remainingCharges = maxCharges - jailbirdItem.TotalChargesPerformed;

                    if (remainingCharges > 1)
                        ev.Player.SendHint($"<color=#00B7EB>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 2);
                    else
                        ev.Player.SendHint($"<color=#C73804>{new string('\n', 10)}{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>", 2);
                }
            }
        }
    }
}