using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Exiled.API.Features;

namespace UsefulHints
{
    public static class UpdateChecker
    {
        private static readonly string RepositoryUrl = "https://api.github.com/repos/Vretu-Dev/UsefulHints/releases/latest";
        private static readonly string PluginPath = UsefulHints.Instance.Config.PluginPath;
        private static readonly string CurrentVersion = UsefulHints.Instance.Version.ToString();
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
            Log.Info("Checking for updates...");
            Task.Run(async () => await CheckForUpdates(true));
        }
        private static async Task CheckForUpdates(bool autoUpdate)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "UpdateChecker");
                try
                {
                    // Dowload info about latest version
                    var response = await client.GetAsync(RepositoryUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Error($"Failed to check for updates: {response.StatusCode} - {response.ReasonPhrase}");
                        return;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var latestVersion = ExtractLatestVersion(content);
                    var downloadUrl = ExtractDownloadUrl(content);

                    if (latestVersion == null || downloadUrl == null)
                    {
                        Log.Error("Failed to parse update information.");
                        return;
                    }

                    // Check Version
                    if (IsNewerVersion(CurrentVersion, latestVersion))
                    {
                        Log.Warn($"A new version is available: {latestVersion} (current: {CurrentVersion})");

                        if (autoUpdate)
                        {
                            Log.Info("Automatic update is enabled. Downloading and applying the update...");
                            UpdatePlugin(downloadUrl);


                            RestartRound();
                        }
                        else
                        {
                            Log.Warn("Automatic update is disabled. Please download the update manually.");
                        }
                    }
                    else
                    {
                        Log.Info("You are using the latest version.");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error while checking for updates: {ex.Message}");
                }
            }
        }
        private static void RestartRound()
        {
            try
            {
                // Restart Command
                string command = "rnr";
                Server.ExecuteCommand(command);
                Log.Info("Round restart initiated.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error while restarting the round: {ex.Message}");
            }
        }
        private static string ExtractLatestVersion(string json)
        {
            var startIndex = json.IndexOf("\"tag_name\":\"") + 12;
            if (startIndex == -1) return null;

            var endIndex = json.IndexOf("\"", startIndex);
            return endIndex == -1 ? null : json.Substring(startIndex, endIndex - startIndex);
        }
        private static string ExtractDownloadUrl(string json)
        {
            var startIndex = json.IndexOf("\"browser_download_url\":\"") + 24;
            if (startIndex == -1) return null;

            var endIndex = json.IndexOf("\"", startIndex);
            return endIndex == -1 ? null : json.Substring(startIndex, endIndex - startIndex);
        }
        private static bool IsNewerVersion(string currentVersion, string latestVersion)
        {
            var currentParts = currentVersion.Split('.');
            var latestParts = latestVersion.Split('.');

            for (int i = 0; i < Math.Min(currentParts.Length, latestParts.Length); i++)
            {
                if (int.Parse(currentParts[i]) < int.Parse(latestParts[i]))
                    return true;

                if (int.Parse(currentParts[i]) > int.Parse(latestParts[i]))
                    return false;
            }

            return latestParts.Length > currentParts.Length;
        }
        private static void UpdatePlugin(string downloadUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var pluginData = client.GetByteArrayAsync(downloadUrl).Result;

                    if (UsefulHints.Instance.Config.EnableBackup)
                    {
                        // Create backup current plugin
                        string backupPath = PluginPath + ".backup";
                    
                        if (File.Exists(PluginPath))
                        {
                            File.Copy(PluginPath, backupPath, overwrite: true);
                            Log.Warn($"Backup created: {backupPath}");
                        }
                    }

                    // Override plugin
                    File.WriteAllBytes(PluginPath, pluginData);
                    Log.Info("Plugin updated successfully. Restart the server to apply changes.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error during plugin update: {ex.Message}");
            }
        }
    }
}