using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarshmallowPortal.Client.Controls;

public class Answer : UserControl
{
    public readonly AnswerViewModel ViewModel = new();

    public string AnswerText
    {
        get => ViewModel.AnswerText;
        set => ViewModel.AnswerText = value;
    }
    
    public bool IsCorrect
    {
        get => ViewModel.IsCorrect;
        set => ViewModel.IsCorrect = value;
    }

    public Answer()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}