using System;
using System.IO;

namespace PhotosForGrandpa.WPF.Config
{
    public class AppConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhotosForGrandpa",
            "config.cfg");

        public const string DefaultZipFileName = "Photos-3-001.zip";
        public const string DefaultGooglePhotosUrl = "https://photos.google.com/login";

        public string ZipFileName { get; set; } = DefaultZipFileName;
        public string GooglePhotosUrl { get; set; } = DefaultGooglePhotosUrl;

        public static AppConfig Load()
        {
            var config = new AppConfig();
            if (!File.Exists(ConfigFilePath))
                return config;

            try
            {
                foreach (var line in File.ReadAllLines(ConfigFilePath))
                {
                    var separatorIndex = line.IndexOf('=');
                    if (separatorIndex <= 0)
                        continue;

                    var key = line.Substring(0, separatorIndex).Trim();
                    var value = line.Substring(separatorIndex + 1).Trim();

                    if (key == nameof(ZipFileName) && !string.IsNullOrWhiteSpace(value))
                        config.ZipFileName = value;
                    else if (key == nameof(GooglePhotosUrl) && !string.IsNullOrWhiteSpace(value))
                        config.GooglePhotosUrl = value;
                }
            }
            catch { }

            return config;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath)!);
                File.WriteAllLines(ConfigFilePath, new[]
                {
                    $"{nameof(ZipFileName)}={ZipFileName}",
                    $"{nameof(GooglePhotosUrl)}={GooglePhotosUrl}"
                });
            }
            catch { }
        }
    }
}
