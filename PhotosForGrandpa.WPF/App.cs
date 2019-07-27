using Mecha.Wpf.Settings;
using PhotosForGrandpa.WPF.Exceptions;
using PhotosForGrandpa.WPF.ViewModels;
using System;

public class App : IApp
{
    public void Init(AppSettings s)
    {
        try
        {
            s.Title = "Foto en video's organiseren";
            s.Window.Width = 400;
            s.Window.Height = 400;
            s.Content = typeof(OrganizerViewModel);
        }
        catch (Exception e) when (!(e is ErrorDialogException))
        {
            //TODO: Log error

            throw new Exception(
                $"Er is iets fout gegaan! {Environment.NewLine}" +
                $"Nodig uw kleinzoon uit voor rijstepap en hij zal het oplossen!");
        }
    }
}