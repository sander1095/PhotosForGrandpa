using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PhotosForGrandpa.WPF
{
    public partial class App : Application
    {
        public static Logger Logger { get; } = new Logger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception);
            MessageBox.Show(e.Exception.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }

    public sealed class Logger
    {
        private readonly string _logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhotosForGrandpa");

        public Logger()
        {
            try
            {
                Directory.CreateDirectory(_logDir);
                CleanupOld();
            }
            catch
            {
            }
        }

        public void Info(string message) => Write("INFO", message);

        public void Error(string message) => Write("ERROR", message);

        public void Error(Exception exception) => Write("ERROR", exception.ToString());

        private void Write(string level, string message)
        {
            try
            {
                var path = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
                File.AppendAllText(path, $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}{Environment.NewLine}");
            }
            catch
            {
            }
        }

        private void CleanupOld()
        {
            var cutoff = DateTime.Now - TimeSpan.FromDays(90);
            foreach (var file in Directory.GetFiles(_logDir, "*.log"))
            {
                try
                {
                    if (File.GetLastWriteTime(file) < cutoff)
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
