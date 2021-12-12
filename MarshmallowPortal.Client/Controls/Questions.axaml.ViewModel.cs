using System;
using System.Collections.Generic;
using MarshmallowPortal.Client.ViewModels;
using ReactiveUI;

namespace MarshmallowPortal.Client.Controls;

public class QuestionsViewModel : ViewModelBase
{
    private string _questionText = null!;
    private int _questionNumber;
    private IEnumerable<Answer> _answers = new List<Answer>();

    public string QuestionText
    {
        get => _questionText;
        set => this.RaiseAndSetIfChanged(ref _questionText, value);
    }

    public string QuestionNumber
    {
        get => _questionNumber.ToString() + '.';
        set => this.RaiseAndSetIfChanged(ref _questionNumber, int.TryParse(value, out var val) ? val : -1);
    }
    
    public IEnumerable<Answer> Answers 
    {
        get => _answers;
        set => this.RaiseAndSetIfChanged(ref _answers, value);
    }
}