using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace UsefulHints.Extensions
{
    public static class UpdateChecker
    {
        private static readonly string RepositoryUrl = "https://api.github.com/repos/Vretu-Dev/UsefulHints/releases/latest";
        private static readonly string PluginPath = Path.Combine(Paths.Plugins, "UsefulHints.dll");
        private static readonly string CurrentVersion = UsefulHints.Instance.Version.ToString();
        private static readonly HttpClient HttpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "User-Agent", "UpdateChecker" } }
        };

        public static void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += WaitingForPlayers;
        }
        public static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
        }

        private static void WaitingForPlayers()
        {
            LogInfo("Checking for updates...");
            Task.Run(() => CheckForUpdates(true));
        }
        private static void LogInfo(string message)
        {
            if (UsefulHints.Instance.Config.EnableLogging)
            {
                Log.Info(message);
            }
        }
        private static void LogWarn(string message)
        {
            if (UsefulHints.Instance.Config.EnableLogging)
            {
                Log.Warn(message);
            }
        }
        private static void LogError(string message)
        {
            if (UsefulHints.Instance.Config.EnableLogging)
            {
                Log.Error(message);
            }
        }
        private static async Task CheckForUpdates(bool autoUpdate)
        {
            try
            {
                var response = await HttpClient.GetAsync(RepositoryUrl);
                if (!response.IsSuccessStatusCode)
                {
                    LogError($"Failed to check for updates: {response.StatusCode} - {response.ReasonPhrase}");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var latestVersion = ExtractLatestVersion(content);
                var downloadUrl = ExtractDownloadUrl(content);

                if (latestVersion == null || downloadUrl == null)
                {
                    LogError("Failed to parse update information.");
                    return;
                }

                if (IsNewerVersion(CurrentVersion, latestVersion))
                {
                    LogWarn($"A new version is available: {latestVersion} (current: {CurrentVersion})");

                    if (autoUpdate)
                    {
                        LogInfo("Automatic update is enabled. Downloading and applying the update...");
                        await UpdatePluginAsync(downloadUrl);
                        RestartRound();
                    }
                    else
                    {
                        LogWarn("Automatic update is disabled. Please download the update manually.");
                    }
                }
                else
                {
                    LogInfo("You are using the latest version.");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error while checking for updates: {ex.Message}");
            }
        }
        private static void RestartRound()
        {
            try
            {
                // Restart Command
                string command = "rnr";
                Server.ExecuteCommand(command);
                LogInfo("Round restart initiated.");
            }
            catch (Exception ex)
            {
                LogError($"Error while restarting the round: {ex.Message}");
            }
        }
        private static string ExtractLatestVersion(string json)
        {
            try
            {
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                return obj["tag_name"]?.ToString();
            }
            catch (Exception ex)
            {
                LogError($"Failed to extract the latest version: {ex.Message}");
                return null;
            }
        }
        private static string ExtractDownloadUrl(string json)
        {
            try
            {
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                return obj["assets"]?[0]?["browser_download_url"]?.ToString();
            }
            catch (Exception ex)
            {
                LogError($"Failed to extract download URL: {ex.Message}");
                return null;
            }
        }
        private static bool IsNewerVersion(string currentVersion, string latestVersion)
        {
            if (Version.TryParse(currentVersion, out var current) && Version.TryParse(latestVersion, out var latest))
            {
                return latest > current;
            }

            LogWarn("Failed to compare versions. Using current version as the latest.");
            return false;
        }
        private static async Task UpdatePluginAsync(string downloadUrl)
        {
            try
            {
                var pluginData = await HttpClient.GetByteArrayAsync(downloadUrl);
                BackupAndWritePlugin(pluginData);
                LogInfo("Plugin updated successfully. Restart the server to apply changes.");
            }
            catch (Exception ex)
            {
                LogError($"Error during plugin update: {ex.Message}");
            }
        }
        private static void BackupAndWritePlugin(byte[] pluginData)
        {
            if (UsefulHints.Instance.Config.EnableBackup)
            {
                string backupPath = PluginPath + ".backup";
                if (File.Exists(PluginPath))
                {
                    File.Copy(PluginPath, backupPath, overwrite: true);
                    LogWarn($"Backup created: {backupPath}");
                }
            }

            File.WriteAllBytes(PluginPath, pluginData);
        }
    }
}