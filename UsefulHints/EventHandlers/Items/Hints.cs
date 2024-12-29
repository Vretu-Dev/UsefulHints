using System;
using System.Linq;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using JailbirdPickup = Exiled.API.Features.Pickups.JailbirdPickup;
using Player = Exiled.API.Features.Player;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Jailbird;
using MEC;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Enum;

namespace UsefulHints.EventHandlers.Items
{
    public static class Hints
    {
        private static readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();
        private static Dictionary<Player, ItemType> activeItems = new Dictionary<Player, ItemType>();
        
        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnGrenadeHurting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpSCP207;
            Exiled.Events.Handlers.Player.ChangingItem += OnEquipSCP207;
            Exiled.Events.Handlers.Player.UsedItem += OnSCP1576Used;
            Exiled.Events.Handlers.Player.ChangedItem += OnSCP1576ChangedItem;
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
            if (!ev.IsAllowed || ev.Amount <= 0.01f || ev.Attacker == null || ev.Player == null || ev.Player == ev.Attacker)
                return;

            if (ev.DamageHandler.Type == DamageType.Explosion)
            {
                float RemainingHealth = ev.Player.Health - ev.Amount;
                if (RemainingHealth > 0 && !ev.Attacker.IsHost)
                {
                    var hint = new DynamicHint
                    {
                        Text = string.Format(UsefulHints.Instance.Config.GrenadeDamageHint, Math.Round(ev.Amount)),
                        TargetY = 700,
                        FontSize = 32,
                    };

                    PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Attacker);
                    playerDisplay.AddHint(hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(hint); });
                }
            }
        }
        // SCP 207 Handler
        private static void OnPickingUpSCP207(PickingUpItemEventArgs ev)
        {
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

            if (ev.Pickup.Type == ItemType.SCP207)
            {
                CustomPlayerEffects.StatusEffectBase scp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.Scp207);

                if (scp207Effect != null)
                {
                    var hint = new DynamicHint
                    {
                        Text = $"<color=#A60C0E>{string.Format(UsefulHints.Instance.Config.Scp207HintMessage, scp207Effect.Intensity)}</color>",
                        TargetY = 800,
                        FontSize = 32,
                    };

                    playerDisplay.AddHint(hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(hint); });
                }
            }
            if (ev.Pickup.Type == ItemType.AntiSCP207)
            {
                CustomPlayerEffects.StatusEffectBase antiscp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.AntiScp207);

                if (antiscp207Effect != null)
                {
                    var hint = new DynamicHint
                    {
                        Text = $"<color=#C53892>{string.Format(UsefulHints.Instance.Config.AntiScp207HintMessage, antiscp207Effect.Intensity)}</color>",
                        TargetY = 800,
                        FontSize = 32,
                    };

                    playerDisplay.AddHint(hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(hint); });
                }
            }
        }
        private static void OnEquipSCP207(ChangingItemEventArgs ev)
        {
            if (UsefulHints.Instance.Config.ShowHintOnEquipItem)
            {
                PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

                if (ev.Item == null)
                {
                    return;
                }
                if (ev.Item.Type == ItemType.SCP207)
                {
                    CustomPlayerEffects.StatusEffectBase scp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.Scp207);

                    if (scp207Effect != null)
                    {
                        var hint = new DynamicHint
                        {
                            Text = $"<color=#A60C0E>{string.Format(UsefulHints.Instance.Config.Scp207HintMessage, scp207Effect.Intensity)}</color>",
                            TargetY = 800,
                            FontSize = 32,
                        };

                        playerDisplay.AddHint(hint);
                        Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(hint); });
                    }
                }
                if (ev.Item.Type == ItemType.AntiSCP207)
                {
                    CustomPlayerEffects.StatusEffectBase antiscp207Effect = ev.Player.ActiveEffects.FirstOrDefault(effect => effect.GetEffectType() == EffectType.AntiScp207);

                    if (antiscp207Effect != null)
                    {
                        var hint = new DynamicHint
                        {
                            Text = $"<color=#C53892>{string.Format(UsefulHints.Instance.Config.AntiScp207HintMessage, antiscp207Effect.Intensity)}</color>",
                            TargetY = 800,
                            FontSize = 32,
                        };

                        playerDisplay.AddHint(hint);
                        Timing.CallDelayed(2f, () => { playerDisplay.RemoveHint(hint);  });
                    }
                }
            }
        }
        // SCP 1576 Handler
        private static readonly Dictionary<Player, DynamicHint> active1576Hints = new Dictionary<Player, DynamicHint>();
        private static void OnSCP1576Used(UsedItemEventArgs ev)
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
                if (active1576Hints.ContainsKey(ev.Player))
                {
                    ev.Player.GetPlayerDisplay().RemoveHint(active1576Hints[ev.Player]);
                    active1576Hints.Remove(ev.Player);
                }

                var coroutine = Timing.RunCoroutine(Scp1576Timer(ev.Player));
                activeCoroutines[ev.Player] = coroutine;
                activeItems[ev.Player] = ev.Item.Type;
            }
        }
        private static void OnSCP1576ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP1576)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);

                if (active1576Hints.ContainsKey(ev.Player))
                {
                    ev.Player.GetPlayerDisplay().RemoveHint(active1576Hints[ev.Player]);
                    active1576Hints.Remove(ev.Player);
                }
            }
        }
        private static IEnumerator<float> Scp1576Timer(Player player)
        {
            float duration = 30f;

            var SCP1576Hint = new DynamicHint()
            {
                TargetY = 900,
                FontSize = 28,
                SyncSpeed = HintSyncSpeed.UnSync
            };

            if (!active1576Hints.ContainsKey(player))
            {
                active1576Hints.Add(player, SCP1576Hint);
            }

            while (duration > 0)
            {
                SCP1576Hint.Text = $"<color=#FFA500>{string.Format(UsefulHints.Instance.Config.Scp1576TimeLeftMessage, (int)duration)}</color>";
                player.GetPlayerDisplay().AddHint(SCP1576Hint);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }

            player.GetPlayerDisplay().RemoveHint(SCP1576Hint);
            active1576Hints.Remove(player);
            activeCoroutines.Remove(player);
        }
        // SCP 268 Handler
        private static readonly Dictionary<Player, DynamicHint> active268Hints = new Dictionary<Player, DynamicHint>();
        private static void OnSCP268Used(UsedItemEventArgs ev)
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
                if (active268Hints.ContainsKey(ev.Player))
                {
                    ev.Player.GetPlayerDisplay().RemoveHint(active268Hints[ev.Player]);
                    active268Hints.Remove(ev.Player);
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

                if (active268Hints.ContainsKey(ev.Player))
                {
                    ev.Player.GetPlayerDisplay().RemoveHint(active268Hints[ev.Player]);
                    active268Hints.Remove(ev.Player);
                }
            }
        }
        private static void OnSCP268ChangedItem(ChangedItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player) && activeItems.ContainsKey(ev.Player) && activeItems[ev.Player] == ItemType.SCP268)
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                activeItems.Remove(ev.Player);

                if (active268Hints.ContainsKey(ev.Player))
                {
                    ev.Player.GetPlayerDisplay().RemoveHint(active268Hints[ev.Player]);
                    active268Hints.Remove(ev.Player);
                }
            }
        }
        private static IEnumerator<float> Scp268Timer(Player player)
        {
            float duration = 15f;

            var SCP268Hint = new DynamicHint()
            {
                TargetY = 900,
                FontSize = 28,
                SyncSpeed = HintSyncSpeed.UnSync
            };

            if (!active268Hints.ContainsKey(player))
            {
                active268Hints.Add(player, SCP268Hint);
            }

            while (duration > 0)
            {
                SCP268Hint.Text = $"<color=purple>{string.Format(UsefulHints.Instance.Config.Scp268TimeLeftMessage, (int)duration)}</color>";
                player.GetPlayerDisplay().AddHint(SCP268Hint);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }

            player.GetPlayerDisplay().RemoveHint(SCP268Hint);
            active268Hints.Remove(player);
            activeCoroutines.Remove(player);
        }
        // SCP 2176 Handler
        private static readonly Dictionary<Player, DynamicHint> active2176Hints = new Dictionary<Player, DynamicHint>();
        private static void OnSCP2176Grenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.Base is Scp2176Projectile)
            {
                if (ev.Player != null)
                {
                    if (activeCoroutines.ContainsKey(ev.Player))
                    {
                        Timing.KillCoroutines(activeCoroutines[ev.Player]);
                        activeCoroutines.Remove(ev.Player);
                    }

                    if (active2176Hints.ContainsKey(ev.Player))
                    {
                        ev.Player.GetPlayerDisplay().RemoveHint(active2176Hints[ev.Player]);
                        active2176Hints.Remove(ev.Player);
                    }

                    var coroutine = Timing.RunCoroutine(Scp2176Timer(ev.Player));
                    activeCoroutines.Add(ev.Player, coroutine);
                }
            }
        }
        private static IEnumerator<float> Scp2176Timer(Player player)
        {
            float duration = 13f;

            var SCP2176Hint = new DynamicHint()
            {
                TargetY = 900,
                FontSize = 28,
                SyncSpeed = HintSyncSpeed.UnSync
            };

            if (!active2176Hints.ContainsKey(player))
            {
                active2176Hints.Add(player, SCP2176Hint);
            }

            while (duration > 0)
            {
                SCP2176Hint.Text = $"<color=#1CAA21>{string.Format(UsefulHints.Instance.Config.Scp2176TimeLeftMessage, (int)duration)}</color>";
                player.GetPlayerDisplay().AddHint(SCP2176Hint);
                yield return Timing.WaitForSeconds(1f);
                duration -= 1f;
            }

            player.GetPlayerDisplay().RemoveHint(SCP2176Hint);
            active2176Hints.Remove(player);
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
                PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

                int maxCharges = 5;
                int remainingCharges = maxCharges - jailbirdPickup.TotalCharges;
                if (remainingCharges > 1)
                {
                    var hint = new DynamicHint
                    {
                        Text = $"<color=#00B7EB>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>",
                        TargetY = 800,
                        FontSize = 32,
                    };

                    playerDisplay.AddHint(hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(hint); });
                }
                else
                {
                    var hint = new DynamicHint
                    {
                        Text = $"<color=#C73804>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>",
                        TargetY = 800,
                        FontSize = 32,
                    };

                    playerDisplay.AddHint(hint);
                    Timing.CallDelayed(4f, () => { playerDisplay.RemoveHint(hint); });
                }
            }
        }
        private static void OnEquipJailbird(ChangingItemEventArgs ev)
        {
            if (UsefulHints.Instance.Config.ShowHintOnEquipItem)
            {
                if (ev.Item == null)
                {
                    return;
                }
                if (ev.Item.Base is JailbirdItem jailbirdItem)
                {
                    int maxCharges = 5;
                    int remainingCharges = maxCharges - jailbirdItem.TotalChargesPerformed;

                    if (remainingCharges > 1)
                    {
                        var hint = new DynamicHint
                        {
                            Text = $"<color=#00B7EB>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>",
                            TargetY = 800,
                            FontSize = 32,
                        };

                        PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);
                        playerDisplay.AddHint(hint);

                        Timing.CallDelayed(2f, () => {
                            playerDisplay.RemoveHint(hint);
                        });
                    }
                    else
                    {
                        var hint = new DynamicHint
                        {
                            Text = $"<color=#C73804>{string.Format(UsefulHints.Instance.Config.JailbirdUseMessage, remainingCharges)}</color>",
                            TargetY = 800,
                            FontSize = 32,
                        };

                        PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);
                        playerDisplay.AddHint(hint);

                        Timing.CallDelayed(2f, () => {
                            playerDisplay.RemoveHint(hint);
                        });
                    }
                }
            }
        }
    }
}