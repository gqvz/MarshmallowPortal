using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarshmallowPortal.Client.Controls;

public class Question : UserControl
{
    public readonly QuestionsViewModel ViewModel = new();

    public string QuestionText
    {
        get => ViewModel.QuestionText;
        set => ViewModel.QuestionText = value;
    }
    
    public IEnumerable<Answer> Answers
    {
        get => ViewModel.Answers;
        set => ViewModel.Answers = value;
    }

    public Question()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}