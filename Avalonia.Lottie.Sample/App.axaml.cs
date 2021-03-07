using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Lottie.Sample.ViewModels;
using Avalonia.Lottie.Sample.Views;
using Avalonia.Markup.Xaml;

namespace Avalonia.Lottie.Sample
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowViewModel = new MainWindowViewModel();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}