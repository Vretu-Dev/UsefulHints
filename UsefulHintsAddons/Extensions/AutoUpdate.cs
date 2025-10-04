using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace UsefulHintsAddons.Extensions
{
    internal static class UpdateChecker
    {
        private const string ApiUrl = "https://api.github.com/repos/Vretu-Dev/UsefulHints/releases/latest";
        private static bool _registered;

        private static Config Cfg => UsefulHintsAddons.Instance?.Config;
        private static string CurrentVersion => UsefulHints.UsefulHints.Instance?.Version.ToString();
        private static string PluginPath => Path.Combine(Paths.Plugins, "UsefulHints.dll");

        private static readonly HttpClient Http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(8)
        };

        static UpdateChecker()
        {
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("UsefulHintsAddons-Updater/1.0");
        }

        public static void Register()
        {
            if (_registered || Cfg == null || !Cfg.EnableAutoUpdate)
                return;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaiting;
            _registered = true;

            if (Cfg.EnableLogging)
                Log.Debug("[Update] UpdateChecker registered.");
        }

        public static void Unregister()
        {
            if (!_registered)
                return;

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaiting;
            _registered = false;

            if (Cfg?.EnableLogging == true)
                Log.Debug("[Update] UpdateChecker unregistered.");
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

                string latest = obj["tag_name"]?.ToString() ?? "";
                string downloadUrl = obj["assets"]?[0]?["browser_download_url"]?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(latest))
                {
                    if (Cfg.EnableLogging)
                        Log.Warn("[Update] Could not parse latest version tag.");
                    return;
                }

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
                        Log.Info("[Update] Download disabled or missing asset URL.");
                        return;
                    }

                    await DownloadAndReplaceAsync(downloadUrl, latest);
                }
                else
                {
                    if (Cfg.EnableLogging)
                        Log.Info("[Update] Already up to date.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("[Update] Update check failed: " + ex.Message);
            }
        }

        private static bool IsNewer(string current, string latest)
        {
            if (Version.TryParse(current, out var c) && Version.TryParse(latest, out var l))
                return l > c;

            return false;
        }

        private static async Task DownloadAndReplaceAsync(string url, string latest)
        {
            try
            {
                Log.Info("[Update] Downloading new UsefulHints version...");
                var data = await Http.GetByteArrayAsync(url);

                if (Cfg.EnableBackup && File.Exists(PluginPath))
                {
                    string backup = PluginPath + ".backup";
                    File.Copy(PluginPath, backup, true);
                    Log.Warn($"[Update] Backup created: {backup}");
                }

                File.WriteAllBytes(PluginPath, data);
                Log.Warn($"[Update] UsefulHints {latest} downloaded.");

                if (Cfg.RestartNextRound)
                {
                    Log.Warn("[Update] Init Restart Next Round");
                    Server.ExecuteCommand("rnr");
                }
            }
            catch (Exception ex)
            {
                Log.Error("[Update] Download failed: " + ex.Message);
            }
        }
    }
}