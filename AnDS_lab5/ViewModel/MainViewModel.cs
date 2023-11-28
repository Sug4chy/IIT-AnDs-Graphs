using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AnDS_lab5.View;

namespace AnDS_lab5.ViewModel;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly MainWindow _window = null!;
    private VertexViewModel? _selectedVertex1;
    private VertexViewModel? _selectedVertex2;
    public ObservableCollection<VertexViewModel> VertexViewModels { get; set; } = new();

    public VertexViewModel? SelectedVertex1
    {
        get => _selectedVertex1;
        set
        {
            _selectedVertex1 = value;
            OnPropertyChanged();
        }
    }

    public VertexViewModel? SelectedVertex2
    {
        get => _selectedVertex2;
        set
        {
            _selectedVertex2 = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand AddVertexCommand { get; } = null!;
    public ICommand AddEdgeCommand { get; } = null!;

    private int _edgeCounter;
    private int _vertexCounter;

    public MainViewModel(MainWindow window)
    {
        _window = window;
        AddVertexCommand = new RelayCommand(AddVertex);
        AddEdgeCommand = new RelayCommand(AddEdge);
    }
    
    public MainViewModel() { }
    
    private void AddVertex(object? o)
    {
        _vertexCounter++;
        var viewModel = new VertexViewModel();
        var ellipse = new Ellipse
        {
            Height = 30,
            Width = 30,
            Fill = Brushes.Aqua,
            Stroke = Brushes.Gray,
            DataContext = viewModel
        };
        ellipse.PreviewMouseDown += _window.UIElement_OnPreviewMouseDown;

        var box = new TextBox
        {
            Height = 20,
            Width = 50,
            TextAlignment = TextAlignment.Center,
            DataContext = viewModel,
        };
        box.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath("Text") });
        viewModel.Ellipse = ellipse;
        viewModel.Box = box;

        viewModel.X = 20;
        viewModel.Y = 20;
        viewModel.Text = _vertexCounter.ToString();
        
        VertexViewModels.Add(viewModel);

        Panel.SetZIndex(box, 1);
        Panel.SetZIndex(ellipse, 1);
        
        _window.CanvasMain.Children.Add(ellipse);
        _window.CanvasMain.Children.Add(box);
    }

    private void AddEdge(object? o)
    {
        if (_edgeCounter == 0)
        {
            _window.ComboBoxes.Visibility = Visibility.Visible;
            _edgeCounter++;
        }
        else
        {
            var edgeViewModel = new EdgeViewModel();
            var edge = new Line
            {
                Stroke = Brushes.Black,
                DataContext = edgeViewModel
            };
            
            edgeViewModel.Vertex1 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
            edgeViewModel.Vertex2 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex2!)];

            edge.SetBinding(Line.X1Property,
                new Binding
                {
                    Path = new PropertyPath("X1"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            edge.SetBinding(Line.X2Property, new Binding
            {
                Path = new PropertyPath("X2"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            edge.SetBinding(Line.Y1Property, new Binding
            {
                Path = new PropertyPath("Y1"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            edge.SetBinding(Line.Y2Property, new Binding
            {
                Path = new PropertyPath("Y2"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            
            Panel.SetZIndex(edge, 0);
            _window.CanvasMain.Children.Add(edge);

            _window.ComboBoxes.Visibility = Visibility.Hidden;
            _edgeCounter--;

            SelectedVertex1 = null;
            SelectedVertex2 = null;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}