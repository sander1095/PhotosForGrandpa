using Mecha.ViewModel.Attributes;
using PhotosForGrandpa.WPF.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace PhotosForGrandpa.WPF.ViewModels
{
    public class OrganizerViewModel
    {
        private const string GooglePhotosDownloadFileName = "Photos.zip";

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

                var filesInArchive = UnzipFile();

                OrganizePhotos(filesInArchive);
                OrganizeVideos(filesInArchive);

                Cleanup();
            }
            catch (Exception e) when (!(e is AppException))
            {
                //TODO: Log error

                throw new Exception(
                    $"Er is iets fout gegaan! {Environment.NewLine}" +
                    $"Nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!");
            }
        }

        private void Validate()
        {
            var newImagesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), FolderName);
            var newVideosFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), FolderName);

            try
            {
                // https://stackoverflow.com/a/3137165/3013479
                _ = Path.GetFullPath(newImagesFolder);
                _ = Path.GetFullPath(newVideosFolder);

                if (FolderName.Contains("/") || FolderName.Contains("\\"))
                {
                    throw new AppException();
                }
            }
            catch
            {
                throw new AppException("De mapnaam mag de volgende karakters niet bevatten: \\ / : * ? \" < > |");
            }
        }

        private IEnumerable<ZipArchiveEntry> UnzipFile()
        {
            var expectedZipFilePath = Path.Combine(Syroot.Windows.IO.KnownFolders.DownloadsLocalized.Path, GooglePhotosDownloadFileName);

            // Ensures that the last character on the extraction path
            // is the directory separator char. 
            // Without this, a malicious zip file could try to traverse outside of the expected
            // extraction path.
            if (!expectedZipFilePath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                expectedZipFilePath += Path.DirectorySeparatorChar;
            }

            using (ZipArchive archive = ZipFile.OpenRead(expectedZipFilePath))
            {
                return archive.Entries;
            }
        }

        private void OrganizePhotos(IEnumerable<ZipArchiveEntry> filesInArchive)
        {
            throw new NotImplementedException();
        }

        private void OrganizeVideos(IEnumerable<ZipArchiveEntry> filesInArchive)
        {
            throw new NotImplementedException();
        }

        private void Cleanup()
        {
            throw new NotImplementedException();
        }

    }
}
