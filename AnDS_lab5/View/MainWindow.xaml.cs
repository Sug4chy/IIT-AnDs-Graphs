using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using AnDS_lab5.ViewModel;

namespace AnDS_lab5.View;

public partial class MainWindow
{
    private Ellipse? _dragObject;
    private Point _offset;
    
    public MainWindow()
    {
        InitializeComponent();
        var viewModel = new MainViewModel(this);
        DataContext = viewModel;
    }

    private void CanvasMain_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragObject is null)
        {
            return;
        }

        var position = e.GetPosition(sender as IInputElement);
        (_dragObject.DataContext as VertexViewModel)!.Y = position.Y - _offset.Y;
        (_dragObject.DataContext as VertexViewModel)!.X = position.X - _offset.X;
    }

    private void CanvasMain_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragObject = null;
        CanvasMain.ReleaseMouseCapture();
    }

    public void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragObject = sender as Ellipse
            ?? throw new ArgumentException("");
        _offset = e.GetPosition(CanvasMain);
        _offset.X -= Canvas.GetLeft(_dragObject);
        _offset.Y -= Canvas.GetTop(_dragObject);
        CanvasMain.CaptureMouse();
    }
}