using System.Windows.Input;

namespace AnDS_lab5.ViewModel;

public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
{
    public bool CanExecute(object? parameter)
        => canExecute is null || canExecute(parameter);

    public void Execute(object? parameter)
        => execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}