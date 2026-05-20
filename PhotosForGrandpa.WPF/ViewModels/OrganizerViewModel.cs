using PhotosForGrandpa.WPF.Config;
using PhotosForGrandpa.WPF.Exceptions;
using PhotosForGrandpa.WPF.Extensions;
using PhotosForGrandpa.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PhotosForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        private static readonly TimeSpan RecentFileThreshold = TimeSpan.FromHours(8);

        public OrganizerViewModel()
        {
            AppConfig.Load();
        }

        public string Intro =>
            $"Dit programma zal ervoor zorgen dat gedownloade foto's van uw {Environment.NewLine}" +
            $"telefoon automatisch gesorteerd worden in de Afbeeldingen {Environment.NewLine}" +
            $"en Video map!{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Stap 1: Download foto's/video's van uw telefoon{Environment.NewLine}" +
            $"Stap 2: Open dit programma{Environment.NewLine}" +
            $"Stap 3: Kies een goede naam voor de map die gemaakt wordt{Environment.NewLine}" +
            $"Stap 4: Klik op de Organiseer knop";

        public string? FolderName { get; set; }

        public void DownloadFotos()
        {
            var config = AppConfig.Load();
            Process.Start(new ProcessStartInfo(config.GooglePhotosUrl)
            {
                UseShellExecute = true
            });
        }

        public void Organiseer()
        {
            try
            {
                var config = AppConfig.Load();
                Validate(config);

                var zipPath = ResolveZipFilePath(config);

                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    var filesInArchive = archive.Entries;

                    OrganizePhotos(filesInArchive, config);
                    OrganizeVideos(filesInArchive, config);
                }

                Cleanup(zipPath);

                UpdateConfigIfAutoDetected(config, zipPath);
            }
            catch (Exception e) when (!(e is ErrorDialogException))
            {
                App.Logger.Error(e);

                ThrowGenericError();
            }
        }

        private static void ThrowGenericError()
        {
            ErrorDialog.ShowError(
                $"Er is iets fout gegaan! {Environment.NewLine}" +
                $"Nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Gebruik de handleiding om de foto's/video's met de hand te organiseren.");
        }

        private string ResolveZipFilePath(AppConfig config)
        {
            var downloadsPath = config.DownloadPath;
            var photosZipFiles = Directory.GetFiles(downloadsPath, "Photos*.zip");

            if (photosZipFiles.Length == 0)
            {
                var recentFiles = Directory.GetFiles(downloadsPath)
                    .OrderByDescending(File.GetCreationTime)
                    .Take(5)
                    .Select(Path.GetFileName);

                ErrorDialog.ShowError(
                    $"Er zijn geen gedownloade fotos gevonden. Controleer dat u fotos heeft gedownload. Als u wél fotos heeft gedownload, neem dan een foto van dit scherm en stuur het door naar uw kleinzoon: {Environment.NewLine}{Environment.NewLine}" +
                    string.Join(Environment.NewLine, recentFiles));
            }

            if (photosZipFiles.Length > 1)
            {
                var fileNames = photosZipFiles.Select(Path.GetFileName);
                App.Logger.Error($"Multiple Photos zip files found: {string.Join(", ", fileNames)}");

                ThrowGenericError();
            }

            if (photosZipFiles.Length == 1 && IsRecentFile(photosZipFiles[0]))
            {
                return photosZipFiles[0];
            }

            return Path.Combine(downloadsPath, config.ZipFileName);
        }

        private static bool IsRecentFile(string path) =>
            DateTime.Now - File.GetCreationTime(path) < RecentFileThreshold;

        private string GetPhotoFolderPath(AppConfig config) =>
            Path.Combine(config.PicturesPath, FolderName ?? string.Empty);

        private string GetVideoFolderPath(AppConfig config) =>
            Path.Combine(config.VideosPath, FolderName ?? string.Empty);

        private void UpdateConfigIfAutoDetected(AppConfig config, string zipPath)
        {
            var zipFileName = Path.GetFileName(zipPath);
            if (string.Equals(zipFileName, config.ZipFileName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            App.Logger.Info($"Google Photos changed zip name from '{config.ZipFileName}' to '{zipFileName}'. Updating config.");
            config.ZipFileName = zipFileName;
            config.Save();
        }

        private void Validate(AppConfig config)
        {
            var folderName = FolderName;
            if (string.IsNullOrWhiteSpace(folderName))
            {
                ErrorDialog.ShowError("Vul eerst een mapnaam in.");
            }

            if (folderName!.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                ErrorDialog.ShowError("De mapnaam mag de volgende karakters niet bevatten: \\ / : * ? \" < > |");
            }

            try
            {
                _ = Path.GetFullPath(GetPhotoFolderPath(config));
                _ = Path.GetFullPath(GetVideoFolderPath(config));
            }
            catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException)
            {
                ErrorDialog.ShowError("De mapnaam mag de volgende karakters niet bevatten: \\ / : * ? \" < > |");
            }
        }

        private void OrganizePhotos(IEnumerable<ZipArchiveEntry> filesInArchive, AppConfig config)
        {
            CopyZipArchiveEntriesToFolder(filesInArchive, GetPhotoFolderPath(config), ".jpg", ".png");
        }

        private void OrganizeVideos(IEnumerable<ZipArchiveEntry> filesInArchive, AppConfig config)
        {
            CopyZipArchiveEntriesToFolder(filesInArchive, GetVideoFolderPath(config), ".mp4");
        }

        private void CopyZipArchiveEntriesToFolder(IEnumerable<ZipArchiveEntry> filesInArchive, string pathToCopyTo, params string[] fileExtensions)
        {
            var files = filesInArchive.GetFilesWithExtension(fileExtensions);

            if (!files.Any())
            {
                return;
            }

            var createdDirectory = Directory.CreateDirectory(pathToCopyTo);

            foreach (var file in files)
            {
                var fullPath = Path.Combine(createdDirectory.FullName, file.Name);
                file.ExtractToFile(fullPath, true);
            }
        }

        private void Cleanup(string zipPath)
        {
            File.Delete(zipPath);
        }
    }
}
