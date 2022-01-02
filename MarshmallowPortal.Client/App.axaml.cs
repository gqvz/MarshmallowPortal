using System.Reflection.Emit;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MarshmallowPortal.Client.ViewModels;
using MarshmallowPortal.Client.Views;
using Splat;

namespace MarshmallowPortal.Client;

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
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}