using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
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
            Timeout = TimeSpan.FromSeconds(8)
        };

        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static Dictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static Task ApplyAsync(string lang)
        {
            if (UsefulHints.UsefulHints.Instance == null)
            {
                Log.Warn("[Trans] Core not ready. Retrying in 2s...");
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
                    Log.Info($"[Trans] Language applied: {lang}");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log.Error($"[Trans] Apply failed: {ex}");
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
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.UserAgent.ParseAdd("UsefulHintsAddons/1.0");
                
                var response = await Http.SendAsync(req, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                File.WriteAllText(file, json);

                var newMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (newMap != null)
                {
                    _map = new Dictionary<string, string>(newMap, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"[Trans] Download failed ({lang}): {ex.Message}");
                if (File.Exists(file))
                {
                    try
                    {
                        string cached = File.ReadAllText(file);
                        var cachedMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(cached);
                        if (cachedMap != null)
                        {
                            _map = new Dictionary<string, string>(cachedMap, StringComparer.OrdinalIgnoreCase);
                            Log.Info("[Trans] Using cached translation file.");
                        }
                    }
                    catch (Exception cacheEx)
                    {
                        Log.Error($"[Trans] Cache read failed: {cacheEx.Message}");
                    }
                }
            }
        }

        private static void MapIntoCore()
        {
            if (CoreCfg == null || _map == null || _map.Count == 0)
                return;

            var properties = typeof(UsefulHints.Config).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.PropertyType != typeof(string))
                    continue;

                if (_map.TryGetValue(prop.Name, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    prop.SetValue(CoreCfg, value);
                }
            }
        }
    }
}