using MarshmallowPortal.Client.ViewModels;
using ReactiveUI;

namespace MarshmallowPortal.Client.Controls;

public class AnswerViewModel : ViewModelBase
{
    private bool _isCorrect = true;
    private string _answerText = "adawdadawd";

    public bool IsCorrect
    {
        get => _isCorrect;
        set => this.RaiseAndSetIfChanged(ref _isCorrect, value);
    }
    
    public string AnswerText
    {
        get => _answerText;
        set => this.RaiseAndSetIfChanged(ref _answerText, value);
    }
}