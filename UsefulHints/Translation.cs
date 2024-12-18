using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exiled.API.Features;
using System;

namespace UsefulHints
{
    public static class TranslationManager
    {
        public static async Task RegisterEvents()
        {
            await InitializeTranslationsAsync();
        }
        private static async Task InitializeTranslationsAsync()
        {
            string language = UsefulHints.Instance.Config.Language;
            await DownloadTranslationsAsync(language);

            // Hint Settings
            UsefulHints.Instance.Config.Scp096LookMessage = GetTranslation("Scp096LookMessage");
            UsefulHints.Instance.Config.Scp268TimeLeftMessage = GetTranslation("Scp268TimeLeftMessage");
            UsefulHints.Instance.Config.Scp2176TimeLeftMessage = GetTranslation("Scp2176TimeLeftMessage");
            UsefulHints.Instance.Config.Scp1576TimeLeftMessage = GetTranslation("Scp1576TimeLeftMessage");
            UsefulHints.Instance.Config.GrenadeDamageHint = GetTranslation("GrenadeDamageHint");
            UsefulHints.Instance.Config.JailbirdUseMessage = GetTranslation("JailbirdUseMessage");
            UsefulHints.Instance.Config.MicroHidEnergyMessage = GetTranslation("MicroHidEnergyMessage");
            UsefulHints.Instance.Config.MicroHidLowEnergyMessage = GetTranslation("MicroHidLowEnergyMessage");
            UsefulHints.Instance.Config.Scp207HintMessage = GetTranslation("Scp207HintMessage");
            UsefulHints.Instance.Config.AntiScp207HintMessage = GetTranslation("AntiScp207HintMessage");
            // Item Warnings
            UsefulHints.Instance.Config.Scp207Warning = GetTranslation("Scp207Warning");
            UsefulHints.Instance.Config.AntiScp207Warning = GetTranslation("AntiScp207Warning");
            UsefulHints.Instance.Config.Scp1853Warning = GetTranslation("Scp1853Warning");
            // Friendly Fire Warnings
            UsefulHints.Instance.Config.FriendlyFireWarning = GetTranslation("FriendlyFireWarning");
            UsefulHints.Instance.Config.DamageTakenWarning = GetTranslation("DamageTakenWarning");
            UsefulHints.Instance.Config.CuffedAttackerWarning = GetTranslation("CuffedAttackerWarning");
            UsefulHints.Instance.Config.CuffedPlayerWarning = GetTranslation("CuffedPlayerWarning");
            // Kill Counter
            UsefulHints.Instance.Config.KillCountMessage = GetTranslation("KillCountMessage");
            // Round Summary
            UsefulHints.Instance.Config.HumanKillMessage = GetTranslation("HumanKillMessage");
            UsefulHints.Instance.Config.ScpKillMessage = GetTranslation("ScpKillMessage");
            UsefulHints.Instance.Config.TopDamageMessage = GetTranslation("TopDamageMessage");
            UsefulHints.Instance.Config.FirstScpKillerMessage = GetTranslation("FirstScpKillerMessage");
            UsefulHints.Instance.Config.EscaperMessage = GetTranslation("EscaperMessage");
            // Teammates
            UsefulHints.Instance.Config.TeammateHintMessage = GetTranslation("TeammateHintMessage");
            UsefulHints.Instance.Config.AloneHintMessage = GetTranslation("AloneHintMessage");

            // Last Human Broadcast
            UsefulHints.Instance.Config.BroadcastForHuman = GetTranslation("BroadcastForHuman");
            UsefulHints.Instance.Config.BroadcastForScp = GetTranslation("BroadcastForScp");
            // Map Broadcast
            UsefulHints.Instance.Config.BroadcastWarningLcz = GetTranslation("BroadcastWarningLcz");
        }

        private static readonly HttpClient HttpClient = new HttpClient();
        private static Dictionary<string, string> Translations = new Dictionary<string, string>();

        private static async Task DownloadTranslationsAsync(string language)
        {
            string pluginDirectory = "/home/container/.config/EXILED/Configs/UsefulHints/Translations";
            string filePath = Path.Combine(pluginDirectory, $"{language}.json");

            try
            {
                if (!Directory.Exists(pluginDirectory))
                {
                    Directory.CreateDirectory(pluginDirectory);
                    Log.Info($"Utworzono folder: {pluginDirectory}");
                }

                string url = $"https://raw.githubusercontent.com/Vretu-Dev/UsefulHints/refs/heads/main/Translations/{language}.json";
                Log.Info($"Pobieranie tłumaczeń z: {url}");
                string content = await HttpClient.GetStringAsync(url);
                File.WriteAllText(filePath, content);
                Log.Info($"Pobrano tłumaczenia dla języka {language} i zapisano w {filePath}");
                Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"Błąd podczas pobierania tłumaczeń: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Log.Error($"Błąd podczas deserializacji pliku tłumaczeń: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"Wystąpił nieoczekiwany błąd: {ex.Message}");
            }
        }
        private static string GetTranslation(string key)
        {
            return Translations.TryGetValue(key, out string value) ? value : key;
        }
    }
}