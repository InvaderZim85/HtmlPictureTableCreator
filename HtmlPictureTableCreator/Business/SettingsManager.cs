using System.IO;
using HtmlPictureTableCreator.DataObjects;
using HtmlPictureTableCreator.Global;
using Newtonsoft.Json;

namespace HtmlPictureTableCreator.Business
{
    public static class SettingsManager
    {
        /// <summary>
        /// Contains the name of the settings file
        /// </summary>
        private const string SettingsFileName = "Settings.json";
        /// <summary>
        /// Loads the settings for the tool
        /// </summary>
        /// <returns>The settings</returns>
        public static HtmlPageSettingsModel LoadSettings()
        {
            var path = Path.Combine(GlobalHelper.GetBaseFolder(), SettingsFileName);

            if (!File.Exists(path))
                return new HtmlPageSettingsModel();

            var jsonString = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<HtmlPageSettingsModel>(jsonString);
        }
        /// <summary>
        /// Saves the settings
        /// </summary>
        /// <param name="settings">The settings</param>
        public static void SaveSettings(HtmlPageSettingsModel settings)
        {
            var path = Path.Combine(GlobalHelper.GetBaseFolder(), SettingsFileName);

            var jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText(path, jsonString);
        }
    }
}
