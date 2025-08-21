using System.Text.Json;
using System.Text.Json.Serialization;

namespace RegexFileCopier
{
    public static class Settings
    {
        public static string InputDirectory { get; set; }
        public static string OutputDirectory { get; set; }
        public static string RegexText { get; set; }

        private record SettingsContainer(string? InputDirectory, string? OutputDirectory, string? RegexText);

        [JsonIgnore]
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings",
            "RegexFileCopier.json");

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
                    RegexText = loaded.RegexText ?? "";
                }
            }
            catch { }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);

            var container = new SettingsContainer(InputDirectory, OutputDirectory, RegexText);

            string json = JsonSerializer.Serialize(container, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Settings.Load();
            ApplicationConfiguration.Initialize();
            Application.Run(new FormRegexFileCopier());
        }
    }
}