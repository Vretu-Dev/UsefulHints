using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace UsefulHintsAddons.Extensions
{
    internal static class UpdateChecker
    {
        private const string ApiUrl = "https://api.github.com/repos/Vretu-Dev/UsefulHints/releases/latest";
        private static bool _registered;
        private static int _downloadingFlag;

        private static Config Cfg => UsefulHintsAddons.Instance?.Config;
        private static string CurrentVersion => UsefulHints.UsefulHints.Instance?.Version?.ToString() ?? "0.0.0";
        private static string CorePluginPath => Path.Combine(Paths.Plugins, "UsefulHints.dll");
        private static string AddonsPluginPath => Path.Combine(Paths.Plugins, "UsefulHints.Addons.dll");

        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        static UpdateChecker()
        {
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("UsefulHintsAddons-Updater/1.0");
            Http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public static void Register()
        {
            if (_registered || Cfg == null || !Cfg.EnableAutoUpdate)
                return;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaiting;
            _registered = true;

            if (Cfg.EnableLogging)
                Log.Debug("[Update] Registered.");
        }

        public static void Unregister()
        {
            if (!_registered)
                return;

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaiting;
            _registered = false;
            Interlocked.Exchange(ref _downloadingFlag, 0);

            if (Cfg?.EnableLogging == true)
                Log.Debug("[Update] Unregistered.");
        }

        private static void OnWaiting()
        {
            _ = Task.Run(CheckAsync);
        }

        private static async Task CheckAsync()
        {
            if (Cfg == null || !Cfg.EnableAutoUpdate)
                return;

            try
            {
                if (Cfg.EnableLogging)
                    Log.Info("[Update] Checking for updates...");

                var response = await Http.GetStringAsync(ApiUrl).ConfigureAwait(false);
                var obj = JObject.Parse(response);

                string rawTag = obj["tag_name"]?.ToString() ?? string.Empty;
                string latest = NormalizeVersion(rawTag);

                if (string.IsNullOrWhiteSpace(latest))
                {
                    Log.Warn("[Update] Could not parse latest version tag.");
                    return;
                }

                if (!IsNewer(CurrentVersion, latest))
                {
                    if (Cfg.EnableLogging)
                        Log.Info($"[Update] Already up to date (v{CurrentVersion}).");
                    return;
                }

                Log.Warn($"[Update] New version available: v{latest} (Current: v{CurrentVersion})");

                if (Cfg.NotifyOnly) return;

                var assets = obj["assets"] as JArray;
                if (assets == null) return;

                string coreUrl = GetAssetUrl(assets, "UsefulHints.dll");
                string addonsUrl = GetAssetUrl(assets, "UsefulHints.Addons.dll");

                if (string.IsNullOrWhiteSpace(coreUrl))
                {
                    Log.Warn("[Update] Core DLL asset (UsefulHints.dll) not found in release. Skipping auto-update.");
                    return;
                }

                if (Interlocked.CompareExchange(ref _downloadingFlag, 1, 0) != 0)
                    return;

                try
                {
                    var tasks = new List<Task<bool>>
                    {
                        DownloadAndReplaceAsync(coreUrl, CorePluginPath, "Core", latest)
                    };

                    if (!string.IsNullOrWhiteSpace(addonsUrl))
                    {
                        tasks.Add(DownloadAndReplaceAsync(addonsUrl, AddonsPluginPath, "Addons", latest));
                    }

                    bool[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
                    bool anyDownloaded = results.Any(r => r);

                    if (anyDownloaded && Cfg.RestartNextRound)
                    {
                        Log.Warn("[Update] Update completed. Issuing 'rnr'...");
                        Server.ExecuteCommand("rnr");
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _downloadingFlag, 0);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Update] Check failed: {ex.Message}");
            }
        }

        private static string GetAssetUrl(JArray assets, string fileName)
        {
            return assets
                ?.FirstOrDefault(a => (a["name"]?.ToString() ?? "").Equals(fileName, StringComparison.OrdinalIgnoreCase))
                ?["browser_download_url"]?.ToString();
        }

        private static string NormalizeVersion(string tag) => tag?.Trim().TrimStart('v', 'V') ?? string.Empty;

        private static bool IsNewer(string current, string latest)
        {
            return Version.TryParse(current, out var vCurrent) && 
                   Version.TryParse(latest, out var vLatest) && 
                   vLatest > vCurrent;
        }

        private static async Task<bool> DownloadAndReplaceAsync(string url, string targetPath, string label, string version)
        {
            try
            {
                Log.Info($"[Update] Downloading {label} v{version}...");
                var data = await Http.GetByteArrayAsync(url).ConfigureAwait(false);

                if (data == null || data.Length == 0)
                {
                    Log.Error($"[Update] {label} download returned empty data.");
                    return false;
                }

                if (Cfg.EnableBackup && File.Exists(targetPath))
                {
                    string backup = targetPath + ".backup";
                    File.Copy(targetPath, backup, true);
                }

                File.WriteAllBytes(targetPath, data);
                Log.Warn($"[Update] {label} updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Update] {label} update failed: {ex.Message}");
                return false;
            }
        }
    }
}