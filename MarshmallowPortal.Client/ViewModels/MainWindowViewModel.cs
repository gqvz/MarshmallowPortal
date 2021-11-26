using ReactiveUI;

namespace MarshmallowPortal.Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool _logInActive;
    private bool _mainWindowEnabled;
    private float _sidebarWidth;

    public bool LogInActive
    {
        get => _logInActive;
        set => this.RaiseAndSetIfChanged(ref _logInActive, value);
    }
    
    public bool MainWindowEnabled
    {
        get => _mainWindowEnabled;
        set => this.RaiseAndSetIfChanged(ref _mainWindowEnabled, value);
    }
    
    public MainWindowViewModel()
    {
        // TODO: Implement
        LogInActive = false;
        MainWindowEnabled = true;
    }
}