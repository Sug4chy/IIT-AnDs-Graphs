using System.Windows.Input;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class MainWindow
{
    private readonly MainViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel(this);
        DataContext = _viewModel;
    }

    private void CanvasMain_OnPreviewMouseMove(object sender, MouseEventArgs e)
        => _viewModel.CanvasMain_OnPreviewMouseMove(sender, e);

    private void CanvasMain_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        => _viewModel.CanvasMain_OnPreviewMouseUp(sender, e);
}