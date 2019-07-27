using System;
using System.Drawing;
using Mecha.Wpf.Settings;
using PhotosForGrandpa.WPF.ViewModels;

public class App : IApp
{
    public void Init(AppSettings s)
    {
        s.Title = "Foto en video's organiseren";
        s.Window.Width = 400;
        s.Window.Height = 400;
        s.Content = typeof(OrganizerViewModel);
    }
}