using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Serilog;
using Splat;
using Splat.Serilog;

namespace MarshmallowPortal.Client;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("log.txt")
            .WriteTo.Console()
            .CreateLogger();
        var listener = new global::SerilogTraceListener.SerilogTraceListener();
        Trace.Listeners.Add(listener);
        IconProvider.Register<FontAwesomeIconProvider>();
        Locator.CurrentMutable.UseSerilogFullLogger();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(LogEventLevel.Debug)
            .UseReactiveUI();
}