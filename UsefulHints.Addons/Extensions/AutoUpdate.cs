using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        private static string CurrentVersion => UsefulHints.UsefulHints.Instance?.Version.ToString() ?? "0.0.0";
        private static string PluginPath => Path.Combine(Paths.Plugins, "UsefulHints.dll");

        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(8)
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
            Task.Run(CheckAsync);
        }

        private static async Task CheckAsync()
        {
            if (Cfg == null || !Cfg.EnableAutoUpdate)
                return;

            try
            {
                if (Cfg.EnableLogging)
                    Log.Info("[Update] Checking for UsefulHints update...");

                string json = await Http.GetStringAsync(ApiUrl);
                var obj = JObject.Parse(json);

                string rawTag = obj["tag_name"]?.ToString() ?? "";
                string latest = NormalizeVersion(rawTag);

                if (string.IsNullOrWhiteSpace(latest))
                {
                    if (Cfg.EnableLogging)
                        Log.Warn("[Update] Could not parse version tag.");
                    return;
                }

                // Znajdź asset *.dll pasujący do UsefulHints
                string downloadUrl = obj["assets"]
                    ?.Where(a =>
                    {
                        var name = a["name"]?.ToString() ?? "";
                        return name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                               name.IndexOf("usefulhints", StringComparison.OrdinalIgnoreCase) >= 0;
                    })
                    .Select(a => a["browser_download_url"]?.ToString())
                    .FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));

                if (IsNewer(CurrentVersion, latest))
                {
                    Log.Warn($"[Update] New version available: {latest} (current {CurrentVersion})");

                    if (Cfg.NotifyOnly)
                    {
                        Log.Info("[Update] NotifyOnly = true (no download).");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(downloadUrl))
                    {
                        Log.Warn("[Update] No valid DLL asset found.");
                        return;
                    }

                    if (Interlocked.CompareExchange(ref _downloadingFlag, 1, 0) != 0)
                    {
                        if (Cfg.EnableLogging)
                            Log.Debug("[Update] Download already in progress.");
                        return;
                    }

                    try
                    {
                        bool ok = await DownloadAndReplaceAsync(downloadUrl, latest);
                        if (ok && Cfg.RestartNextRound)
                        {
                            Log.Warn("[Update] RestartNextRound = true -> issuing 'rnr' now.");
                            try
                            {
                                Server.ExecuteCommand("rnr");
                            }
                            catch (Exception ex)
                            {
                                Log.Error("[Update] Failed to execute 'rnr': " + ex.Message);
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _downloadingFlag, 0);
                    }
                }
                else
                {
                    if (Cfg.EnableLogging)
                        Log.Info("[Update] Already up to date.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("[Update] Check failed: " + ex.Message);
            }
        }

        private static string NormalizeVersion(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return string.Empty;

            tag = tag.Trim();
            if (tag.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                tag = tag.Substring(1);
            return tag;
        }

        private static bool IsNewer(string current, string latest)
        {
            if (Version.TryParse(current, out var c) && Version.TryParse(latest, out var l))
                return l > c;
            return false;
        }

        private static async Task<bool> DownloadAndReplaceAsync(string url, string latest)
        {
            try
            {
                Log.Info($"[Update] Downloading UsefulHints {latest}...");
                var data = await Http.GetByteArrayAsync(url);

                if (data == null || data.Length == 0)
                {
                    Log.Error("[Update] Downloaded file is empty.");
                    return false;
                }

                if (Cfg.EnableBackup && File.Exists(PluginPath))
                {
                    string backup = PluginPath + ".backup";
                    File.Copy(PluginPath, backup, true);
                    Log.Warn($"[Update] Backup created: {backup}");
                }

                File.WriteAllBytes(PluginPath, data);
                Log.Warn($"[Update] UsefulHints {latest} downloaded. FULL server process restart required to load new DLL.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("[Update] Download failed: " + ex.Message);
                return false;
            }
        }
    }
}