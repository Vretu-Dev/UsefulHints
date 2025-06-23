using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Exiled.API.Features;

namespace UsefulHints.Extensions
{
    public static class TranslationManager
    {
        private static Config Config => UsefulHints.Instance.Config;

        public static async Task RegisterEvents()
        {
            await InitializeTranslationsAsync();
        }
        private static async Task InitializeTranslationsAsync()
        {
            string language = Config.Language.ToLower();
            await DownloadTranslationsAsync(language);

            // Hint Settings
            Config.Scp096LookMessage = GetTranslation("Scp096LookMessage");
            Config.Scp268TimeLeftMessage = GetTranslation("Scp268TimeLeftMessage");
            Config.Scp2176TimeLeftMessage = GetTranslation("Scp2176TimeLeftMessage");
            Config.Scp1576TimeLeftMessage = GetTranslation("Scp1576TimeLeftMessage");
            Config.GrenadeDamageHint = GetTranslation("GrenadeDamageHint");
            Config.JailbirdUseMessage = GetTranslation("JailbirdUseMessage");
            Config.Scp207HintMessage = GetTranslation("Scp207HintMessage");
            Config.AntiScp207HintMessage = GetTranslation("AntiScp207HintMessage");
            // Item Warnings
            Config.Scp207Warning = GetTranslation("Scp207Warning");
            Config.AntiScp207Warning = GetTranslation("AntiScp207Warning");
            Config.Scp1853Warning = GetTranslation("Scp1853Warning");
            // Friendly Fire Warnings
            Config.FriendlyFireWarning = GetTranslation("FriendlyFireWarning");
            Config.DamageTakenWarning = GetTranslation("DamageTakenWarning");
            Config.CuffedAttackerWarning = GetTranslation("CuffedAttackerWarning");
            Config.CuffedPlayerWarning = GetTranslation("CuffedPlayerWarning");
            // Kill Counter
            Config.KillCountMessage = GetTranslation("KillCountMessage");
            // Round Summary
            Config.HumanKillMessage = GetTranslation("HumanKillMessage");
            Config.ScpKillMessage = GetTranslation("ScpKillMessage");
            Config.TopDamageMessage = GetTranslation("TopDamageMessage");
            Config.FirstScpKillerMessage = GetTranslation("FirstScpKillerMessage");
            Config.EscaperMessage = GetTranslation("EscaperMessage");
            // Teammates
            Config.TeammateHintMessage = GetTranslation("TeammateHintMessage");
            Config.AloneHintMessage = GetTranslation("AloneHintMessage");

            // Last Human Broadcast
            Config.BroadcastForHuman = GetTranslation("BroadcastForHuman");
            Config.BroadcastForScp = GetTranslation("BroadcastForScp");
            // Map Broadcast
            Config.BroadcastWarningLcz = GetTranslation("BroadcastWarningLcz");
        }

        private static readonly HttpClient HttpClient = new HttpClient();
        private static Dictionary<string, string> Translations = new Dictionary<string, string>();

        private static async Task DownloadTranslationsAsync(string language)
        {
            string pluginDirectory = Path.Combine(Paths.IndividualTranslations, "UsefulHints");
            string filePath = Path.Combine(pluginDirectory, $"{language}.json");

            try
            {
                if (!Directory.Exists(pluginDirectory))
                {
                    Directory.CreateDirectory(pluginDirectory);
                    Log.Info($"Folder created: {pluginDirectory}");
                }

                string url = $"https://raw.githubusercontent.com/Vretu-Dev/UsefulHints/refs/heads/hsm-version/Translations/{language}.json";
                Log.Debug($"Downloading translations from: {url}");
                string content = await HttpClient.GetStringAsync(url);
                File.WriteAllText(filePath, content);
                Log.Debug($"Translations for language {language} have been downloaded and saved to {filePath}");
                Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"Error during downloading translations: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Log.Error($"Error during translation file deserialization: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred: {ex.Message}");
            }
        }
        private static string GetTranslation(string key)
        {
            return Translations.TryGetValue(key, out string value) ? value : key;
        }
    }
}