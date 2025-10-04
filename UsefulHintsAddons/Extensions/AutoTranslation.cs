using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Newtonsoft.Json;

namespace UsefulHintsAddons.Extensions
{
    public static class TranslationManager
    {
        private static Config AddonsCfg => UsefulHintsAddons.Instance?.Config;
        private static UsefulHints.Config CoreCfg => UsefulHints.UsefulHints.Instance?.Config;

        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(6)
        };

        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static Dictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Public call used by plugin OnEnabled or language command.
        /// </summary>
        public static Task ApplyAsync(string lang)
        {
            if (UsefulHints.UsefulHints.Instance == null)
            {
                Log.Warn(" Core not ready. Retrying in 2s.");
                Timing.RunCoroutine(Retry(lang));
                return Task.CompletedTask;
            }
            return InternalApplyAsync(lang);
        }

        public static void CancelPending()
        {
            try { _cts.Cancel(); } catch { }
        }

        private static IEnumerator<float> Retry(string lang)
        {
            yield return Timing.WaitForSeconds(2f);
            _ = ApplyAsync(lang);
        }

        private static async Task InternalApplyAsync(string lang)
        {
            if (AddonsCfg == null)
                return;

            await Gate.WaitAsync();
            try
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
                var ct = _cts.Token;

                lang = (lang ?? "en").Trim().ToLowerInvariant();
                AddonsCfg.Language = lang;

                await Download(lang, ct);
                MapIntoCore();
                if (AddonsCfg.EnableLogging)
                    Log.Info($"Language applied: {lang}");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.Error("Translation apply failed: " + ex.Message);
            }
            finally
            {
                Gate.Release();
            }
        }

        private static async Task Download(string lang, CancellationToken ct)
        {
            string dir = Path.Combine(Paths.IndividualTranslations, "UsefulHints");
            Directory.CreateDirectory(dir);
            string file = Path.Combine(dir, $"{lang}.json");
            string url = $"https://raw.githubusercontent.com/Vretu-Dev/UsefulHints/refs/heads/main/Translations/{lang}.json";

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.UserAgent.ParseAdd("UsefulHintsAddons/1.0");
                string json = await (await Http.SendAsync(req, ct)).Content.ReadAsStringAsync();
                File.WriteAllText(file, json);
                _map = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                       ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Log.Warn($"Download failed ({lang}): {ex.Message}");
                if (File.Exists(file))
                {
                    try
                    {
                        string cached = File.ReadAllText(file);
                        _map = JsonConvert.DeserializeObject<Dictionary<string, string>>(cached)
                               ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        Log.Warn("Using cached translation file.");
                    }
                    catch (Exception cacheEx)
                    {
                        Log.Error("Cache read failed: " + cacheEx.Message);
                        _map.Clear();
                    }
                }
                else
                {
                    _map.Clear();
                }
            }
        }

        private static string T(string key) => _map.TryGetValue(key, out var v) ? v : key;

        private static void MapIntoCore()
        {
            if (CoreCfg == null || _map.Count == 0)
                return;

            CoreCfg.Scp096LookMessage = T("Scp096LookMessage");
            CoreCfg.Scp268TimeLeftMessage = T("Scp268TimeLeftMessage");
            CoreCfg.Scp2176TimeLeftMessage = T("Scp2176TimeLeftMessage");
            CoreCfg.Scp1576TimeLeftMessage = T("Scp1576TimeLeftMessage");
            CoreCfg.GrenadeDamageHint = T("GrenadeDamageHint");
            CoreCfg.JailbirdUseMessage = T("JailbirdUseMessage");
            CoreCfg.Scp207HintMessage = T("Scp207HintMessage");
            CoreCfg.AntiScp207HintMessage = T("AntiScp207HintMessage");
            CoreCfg.Scp207Warning = T("Scp207Warning");
            CoreCfg.AntiScp207Warning = T("AntiScp207Warning");
            CoreCfg.Scp1853Warning = T("Scp1853Warning");
            CoreCfg.FriendlyFireWarning = T("FriendlyFireWarning");
            CoreCfg.DamageTakenWarning = T("DamageTakenWarning");
            CoreCfg.CuffedAttackerWarning = T("CuffedAttackerWarning");
            CoreCfg.CuffedPlayerWarning = T("CuffedPlayerWarning");
            CoreCfg.KillCountMessage = T("KillCountMessage");
            CoreCfg.HumanKillMessage = T("HumanKillMessage");
            CoreCfg.ScpKillMessage = T("ScpKillMessage");
            CoreCfg.TopDamageMessage = T("TopDamageMessage");
            CoreCfg.FirstScpKillerMessage = T("FirstScpKillerMessage");
            CoreCfg.EscaperMessage = T("EscaperMessage");
            CoreCfg.TeammateHintMessage = T("TeammateHintMessage");
            CoreCfg.AloneHintMessage = T("AloneHintMessage");
            CoreCfg.BroadcastForHuman = T("BroadcastForHuman");
            CoreCfg.BroadcastForScp = T("BroadcastForScp");
            CoreCfg.BroadcastWarningLcz = T("BroadcastWarningLcz");
        }
    }
}