using System.Collections.Generic;
using MarshmallowPortal.Client.ViewModels;
using ReactiveUI;

namespace MarshmallowPortal.Client.Controls;

public class SectionViewModel : ViewModelBase
{
    private IEnumerable<Question> _questions = new List<Question>();
    private string _headerText = "Section F";

    public IEnumerable<Question> Questions
    {
        get => _questions;
        set => this.RaiseAndSetIfChanged(ref _questions, value);
    }

    public string HeaderText
    {
        get => _headerText;
        set => this.RaiseAndSetIfChanged(ref _headerText, value);
    }
}