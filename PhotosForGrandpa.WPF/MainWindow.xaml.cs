using System;
using System.Windows;
using PhotosForGrandpa.WPF.Exceptions;
using PhotosForGrandpa.WPF.ViewModels;

namespace PhotosForGrandpa.WPF
{
    public partial class MainWindow : Window
    {
        private OrganizerViewModel ViewModel { get; } = new OrganizerViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += (_, _) => FolderNameTextBox.Focus();
        }

        private void Organiseer_DownloadFotos(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.DownloadFotos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Organiseer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Organiseer();

                MessageBox.Show(
                    "De foto's en/of video's zijn te vinden in de gekozen map!",
                    "Klaar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (ErrorDialogException ex)
            {
                MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
