using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureAggregator
{
    public class FeedbackEventArgs : EventArgs { public string Feedback { get; set; } }

    public static class Settings
    {
        public static string InputPftxs { get; set; }
        public static string OutputDirectory { get; set; }
        public static string TextureDirectory { get; set; }

        private record SettingsContainer(string? InputPftxs, string? OutputDirectory, string? TextureDirectory);

        [JsonIgnore]
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings",
            "TextureAggregator.json");

        public static void Load()
        {
            if (!File.Exists(ConfigPath))
                return;

            try
            {
                string json = File.ReadAllText(ConfigPath);
                var loaded = JsonSerializer.Deserialize<SettingsContainer>(json);
                if (loaded != null)
                {
                    InputPftxs = loaded.InputPftxs ?? "";
                    OutputDirectory = loaded.OutputDirectory ?? "";
                    TextureDirectory = loaded.TextureDirectory ?? "";
                }
            }
            catch { }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);

            var container = new SettingsContainer(InputPftxs, OutputDirectory, TextureDirectory);

            string json = JsonSerializer.Serialize(container, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormAggregator());
        }
    }
}
