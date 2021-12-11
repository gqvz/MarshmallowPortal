using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarshmallowPortal.Client.Controls;

public class Question : UserControl
{
    
    
    public string QuestionText { get; set; } = "the oauth2 wrapper took me way longer than it should have";
    public string QuestionNumber { get; set; } = "0.";
    public Question()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}