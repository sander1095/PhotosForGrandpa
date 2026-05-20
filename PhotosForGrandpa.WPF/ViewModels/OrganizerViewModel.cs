using Mecha.ViewModel.Attributes;
using PhotosForGrandpa.WPF.Config;
using PhotosForGrandpa.WPF.Exceptions;
using PhotosForGrandpa.WPF.Extensions;
using PhotosForGrandpa.WPF.Helpers;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PhotosForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        private static readonly TimeSpan RecentFileThreshold = TimeSpan.FromHours(8);

        private readonly AppConfig _config;

        public OrganizerViewModel()
        {
            _config = AppConfig.Load();
        }

        private string PhotoFolderPath => Path.Combine(KnownFolders.Pictures.Path, FolderName);
        private string VideoFolderPath => Path.Combine(KnownFolders.Videos.Path, FolderName);

        [Readonly]
        public virtual string Intro =>
            $"Dit programma zal ervoor zorgen dat gedownloade foto's van uw {Environment.NewLine}" +
            $"telefoon automatisch gesorteerd worden in de Afbeeldingen {Environment.NewLine}" +
            $"en Video map!{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Stap 1: Download foto's/video's van uw telefoon via de website op uw bureaublad{Environment.NewLine}" +
            $"Stap 2: Wacht tot het downloaden voltooid is. Dit staat rechtsbovenin." +
            $"Stap 3: Open dit programma{Environment.NewLine}" +
            $"Stap 4: Kies een goede naam voor de map die gemaakt moet worden{Environment.NewLine}" +
            $"Stap 5: Klik op de Organiseer knop";

        [TextInput("Mapnaam", Description = "Gran Canaria 2019", Mandatory = true)]
        public virtual string FolderName { get; set; }

        [Action]
        public void DownloadFotos()
        {
            System.Diagnostics.Process.Start(_config.GooglePhotosUrl);
        }
        [Action]
        [Message("Klaar", "De foto's en/of video's zijn te vinden in de gekozen map!")]
        public void Organiseer()
        {
            try
            {
                Validate();

                var zipPath = ResolveZipFilePath();

                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    var filesInArchive = archive.Entries;

                    OrganizePhotos(filesInArchive);
                    OrganizeVideos(filesInArchive);
                }

                Cleanup(zipPath);

                UpdateConfigIfAutoDetected(zipPath);
            }
            catch (Exception e) when (!(e is ErrorDialogException))
            {
                App.Logger.Error(e);

                ThrowGenericError();
                return;
            }
        }

        private static void ThrowGenericError()
        {
            throw new Exception(
                $"Er is iets fout gegaan! {Environment.NewLine}" +
                $"Nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Gebruik de handleiding om de foto's/video's met de hand te organiseren.");
        }

        private string ResolveZipFilePath()
        {
            var downloadsPath = KnownFolders.Downloads.Path;
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
                ErrorDialog.ShowError(
                    $"Er zijn meerdere zip-bestanden gevonden die beginnen met 'Photos' in de Downloads map.{Environment.NewLine}" +
                    $"Verwijder alle bestanden behalve het bestand dat u wilt gebruiken.");
            }

            if (photosZipFiles.Length == 1 && IsRecentFile(photosZipFiles[0]))
            {
                return photosZipFiles[0];
            }

            return Path.Combine(downloadsPath, _config.ZipFileName);
        }

        private static bool IsRecentFile(string path) =>
            DateTime.Now - File.GetCreationTime(path) < RecentFileThreshold;

        private void UpdateConfigIfAutoDetected(string zipPath)
        {
            var zipFileName = Path.GetFileName(zipPath);
            if (string.Equals(zipFileName, _config.ZipFileName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            App.Logger.Info($"Google Photos changed zip name from '{_config.ZipFileName}' to '{zipFileName}'. Updating config.");
            _config.ZipFileName = zipFileName;
            _config.Save();
        }

        private void Validate()
        {
            try
            {
                // https://stackoverflow.com/a/3137165/3013479
                _ = Path.GetFullPath(PhotoFolderPath);
                _ = Path.GetFullPath(VideoFolderPath);

                if (FolderName.Contains("/") || FolderName.Contains("\\"))
                {
                    throw new ErrorDialogException();
                }
            }
            catch
            {
                ErrorDialog.ShowError("De mapnaam mag de volgende karakters niet bevatten: \\ / : * ? \" < > |");
            }
        }

        private void OrganizePhotos(IEnumerable<ZipArchiveEntry> filesInArchive)
        {
            CopyZipArchiveEntriesToFolder(filesInArchive, PhotoFolderPath, ".jpg", ".png");
        }

        private void OrganizeVideos(IEnumerable<ZipArchiveEntry> filesInArchive)
        {
            CopyZipArchiveEntriesToFolder(filesInArchive, VideoFolderPath, ".mp4");
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
