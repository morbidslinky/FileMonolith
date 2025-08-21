using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileProliferator
{
    public class FeedbackEventArgs : EventArgs { public string Feedback { get; set; } }

    public static class Settings
    {
        public static string InputDirectory { get; set; }
        public static string OutputDirectory { get; set; }
        public static string ReferenceDirectory { get; set; }
        public static string TextureDirectory { get; set; }

        private record SettingsContainer(string? InputDirectory, string? OutputDirectory, string? ReferenceDirectory, string? TextureDirectory);

        [JsonIgnore]
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings",
            "FileProliferator.json");

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
                    InputDirectory = loaded.InputDirectory ?? "";
                    OutputDirectory = loaded.OutputDirectory ?? "";
                    ReferenceDirectory = loaded.ReferenceDirectory ?? "";
                    TextureDirectory = loaded.TextureDirectory ?? "";
                }
            }
            catch { }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);

            var container = new SettingsContainer(InputDirectory, OutputDirectory, ReferenceDirectory, TextureDirectory);

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
            Application.Run(new FormProliferator());
        }
    }
}
