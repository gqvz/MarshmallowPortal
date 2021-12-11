using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MarshmallowPortal.Client.Views;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void AddQuestionToggle_OnChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton) return;
        toggleButton.Flyout.ShowAt(toggleButton);
    }
    
    private void AddQuestionToggle_OnUnChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton) return;
        toggleButton.Flyout.Hide();
    }

    private void FlyoutBase_OnClosing(object? sender, EventArgs eventArgs)
    {
        if (sender is not Flyout {Target: ToggleButton btn}) return;
        btn.IsChecked = false;
    }
}