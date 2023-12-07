using System.Windows.Input;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class OrientedRedactorWindow
{
    private readonly OrientedRedactorViewModel _viewModel;
    
    public OrientedRedactorWindow()
    {
        InitializeComponent();
        _viewModel = new OrientedRedactorViewModel(this);
        DataContext = _viewModel;
    }
    
    private void CanvasMain_OnPreviewMouseMove(object sender, MouseEventArgs e)
        => _viewModel.CanvasMain_OnPreviewMouseMove(sender, e);

    private void CanvasMain_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        => _viewModel.CanvasMain_OnPreviewMouseUp(sender, e);
}