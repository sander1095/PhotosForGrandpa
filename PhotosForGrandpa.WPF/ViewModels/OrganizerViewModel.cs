using Mecha.ViewModel.Attributes;
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
        private string ZipFileFolderPath => Path.Combine(KnownFolders.Downloads.Path, "Photos.zip");
        private string PhotoFolderPath => Path.Combine(KnownFolders.Pictures.Path, FolderName);
        private string VideoFolderPath => Path.Combine(KnownFolders.Videos.Path, FolderName);

        [Readonly]
        public virtual string Intro =>
            $"Dit programma zal ervoor zorgen dat gedownloade foto's van uw {Environment.NewLine}" +
            $"telefoon automatisch gesorteerd worden in de Afbeeldingen {Environment.NewLine}" +
            $"en Video map!{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Stap 1: Download foto's/video's van uw telefoon{Environment.NewLine}" +
            $"Stap 2: Open dit programma{Environment.NewLine}" +
            $"Stap 3: Kies een goede naam voor de map die gemaakt {Environment.NewLine}" +
            $"Stap 4: Klik op de Organiseer knop";

        [TextInput("Mapnaam", Description = "Gran Canaria 2019", Mandatory = true)]
        public virtual string FolderName { get; set; }

        [Action]
        [Message("Klaar", "De foto's en/of video's zijn te vinden in de gekozen map!")]
        public void Organiseer()
        {
            try
            {
                Validate();

                using (var archive = ZipFile.OpenRead(ZipFileFolderPath))
                {
                    var filesInArchive = archive.Entries;

                    OrganizePhotos(filesInArchive);
                    OrganizeVideos(filesInArchive);
                }

                Cleanup();
            }
            catch (Exception e) when (!(e is ErrorDialogException))
            {
                //TODO: Log error
                App.Logger.Error(e);

                throw new Exception(
                    $"Er is iets fout gegaan! {Environment.NewLine}" +
                    $"Nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!");
            }
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

        private void Cleanup()
        {
            File.Delete(ZipFileFolderPath);
        }
    }
}
