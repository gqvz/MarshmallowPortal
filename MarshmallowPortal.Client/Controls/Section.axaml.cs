using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarshmallowPortal.Client.Controls;

public class Section : UserControl
{
    public readonly SectionViewModel ViewModel = new();

    public string HeaderText
    {
        get => ViewModel.HeaderText;
        set => ViewModel.HeaderText = value;
    }
    
    public IEnumerable<Question> Questions
    {
        get => ViewModel.Questions;
        set => ViewModel.Questions = value;
    }

    public Section()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}