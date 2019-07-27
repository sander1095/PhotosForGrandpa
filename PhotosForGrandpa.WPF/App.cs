using LLibrary;
using Mecha.Wpf.Settings;
using PhotosForGrandpa.WPF.ViewModels;
using Syroot.Windows.IO;
using System;
using System.IO;

public class App : IApp
{
    public static L Logger { get; } = new L(
        deleteOldFiles: TimeSpan.FromDays(90),
        directory: Path.Combine(KnownFolders.LocalAppData.Path, "PhotosForGrandpa"));

    public void Init(AppSettings s)
    {
        s.Title = "Foto en video's organiseren";
        s.Window.Width = 400;
        s.Window.Height = 325;
        s.Content = typeof(OrganizerViewModel);
    }
}