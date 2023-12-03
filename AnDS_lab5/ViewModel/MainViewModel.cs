using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AnDS_lab5.Model;
using AnDS_lab5.Service;
using AnDS_lab5.View;

namespace AnDS_lab5.ViewModel;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private EditMode _mode;
    private readonly MainWindow _window = null!;
    private VertexViewModel? _selectedVertex1;
    private VertexViewModel? _selectedVertex2;
    private int _edgeCounter;
    private int _vertexCounter;
    private Ellipse? _dragObject;
    private Point _offset;
    private readonly JsonFileService _fileService = new();
    private readonly DefaultDialogService _dialogService = new();
    private string _filename = "";

    public ObservableCollection<VertexViewModel> VertexViewModels { get; } = new();

    public string Filename
    {
        get => _filename;
        set
        {
            _filename = value;
            OnPropertyChanged();
        }
    }

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

    private EditMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddVertexCommand { get; } = null!;
    public ICommand AddEdgeCommand { get; } = null!;
    public ICommand SetCreatingModeCommand { get; } = null!;
    public ICommand SetDeletingModeCommand { get; } = null!;
    public ICommand SaveToFileCommand { get; } = null!;
    public ICommand OpenFromFileCommand { get; } = null!;

    public MainViewModel(MainWindow window)
    {
        _window = window;
        AddVertexCommand = new RelayCommand(AddVertex, _ => Mode == EditMode.Add);
        AddEdgeCommand = new RelayCommand(AddEdge, _ => Mode == EditMode.Add);
        SetCreatingModeCommand = new RelayCommand(SetCreatingMode, _ => Mode == EditMode.Remove);
        SetDeletingModeCommand = new RelayCommand(SetDeletingMode, _ => Mode == EditMode.Add);
        SaveToFileCommand = new RelayCommand(SaveToFile);
        OpenFromFileCommand = new RelayCommand(OpenFromFile);
    }

    public MainViewModel()
    {
    }

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
        ellipse.PreviewMouseDown += UIElement_OnPreviewMouseDown;

        var box = new TextBox
        {
            Height = 20,
            Width = 50,
            TextAlignment = TextAlignment.Center,
            DataContext = viewModel,
        };
        box.SetBinding(TextBox.TextProperty, new Binding
        {
            Path = new PropertyPath("Text"),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
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

    private void CreateVertexViewModel(string content, double x, double y)
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
        ellipse.PreviewMouseDown += UIElement_OnPreviewMouseDown;

        var box = new TextBox
        {
            Height = 20,
            Width = 50,
            TextAlignment = TextAlignment.Center,
            DataContext = viewModel,
        };
        box.SetBinding(TextBox.TextProperty, new Binding
        {
            Path = new PropertyPath("Text"),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        viewModel.Ellipse = ellipse;
        viewModel.Box = box;

        viewModel.X = x;
        viewModel.Y = y;
        viewModel.Text = content;

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
            if (SelectedVertex1!.Equals(SelectedVertex2!))
            {
                var edgeViewModel = new CircleEdgeViewModel();
                var edge = new Ellipse
                {
                    Height = 50,
                    Width = 50,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black,
                    DataContext = edgeViewModel
                };

                edgeViewModel.Vertex = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
                edgeViewModel.Ellipse = edge;

                Panel.SetZIndex(edge, 0);
                _window.CanvasMain.Children.Add(edge);
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
                edgeViewModel.Line = edge;

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
            }

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

    private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (Mode == EditMode.Add)
        {
            _dragObject = sender as Ellipse
                          ?? throw new ArgumentException("");
            _offset = e.GetPosition(_window.CanvasMain);
            _offset.X -= Canvas.GetLeft(_dragObject) - 10;
            _offset.Y -= Canvas.GetTop(_dragObject);
            _window.CanvasMain.CaptureMouse();
        }
        else
        {
            switch (sender)
            {
                case Ellipse ellipse:
                {
                    var vertex = VertexViewModels.First(vertex => vertex.Ellipse.Equals(ellipse));
                    _window.CanvasMain.Children.Remove(vertex.Ellipse);
                    _window.CanvasMain.Children.Remove(vertex.Box);
                    var edges = vertex.Edges.Select(tuple => tuple.Item1).ToList();

                    foreach (var edge in edges)
                    {
                        if (edge is CircleEdgeViewModel circleEdge)
                        {
                            _window.CanvasMain.Children.Remove(circleEdge.Ellipse);
                        }

                        _window.CanvasMain.Children.Remove(edge.Line);
                    }

                    foreach (var vertexViewModel in VertexViewModels)
                    {
                        vertexViewModel.Edges =
                            vertexViewModel.Edges.Where(tuple => !edges.Contains(tuple.Item1)).ToList();
                    }

                    VertexViewModels.Remove(vertex);
                    break;
                }
                case Line line:
                {
                    _window.CanvasMain.Children.Remove(line);
                    foreach (var vertexViewModel in VertexViewModels)
                    {
                        vertexViewModel.Edges = vertexViewModel.Edges
                            .Where(tuple => !tuple.Item1.Line.Equals(line))
                            .ToList();
                    }

                    break;
                }
            }

            _dragObject = null;
            _window.CanvasMain.ReleaseMouseCapture();
        }
    }

    private void CreateEdgeViewModel()
    {
        if (SelectedVertex1!.Equals(SelectedVertex2!))
        {
            var edgeViewModel = new CircleEdgeViewModel();
            var edge = new Ellipse
            {
                Height = 50,
                Width = 50,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Black,
                DataContext = edgeViewModel
            };

            edgeViewModel.Vertex = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
            edgeViewModel.Ellipse = edge;

            Panel.SetZIndex(edge, 0);
            _window.CanvasMain.Children.Add(edge);
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
            edgeViewModel.Line = edge;

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
        }
    }

    public void CanvasMain_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragObject is null)
        {
            return;
        }

        var position = e.GetPosition(sender as IInputElement);
        (_dragObject.DataContext as VertexViewModel)!.Y = position.Y - _offset.Y;
        (_dragObject.DataContext as VertexViewModel)!.X = position.X - _offset.X;
    }

    public void CanvasMain_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragObject = null;
        _window.CanvasMain.ReleaseMouseCapture();
    }

    private void SetCreatingMode(object? o)
        => Mode = EditMode.Add;

    private void SetDeletingMode(object? o)
        => Mode = EditMode.Remove;

    private void SaveToFile(object? o)
    {
        try
        {
            if (!_dialogService.SaveFileDialog())
            {
                return;
            }

            var edges = (from vertexViewModel in VertexViewModels
                from tuple
                    in vertexViewModel.Edges
                select tuple.Item1.ToEdge()).Distinct(new EdgeComparer()).ToList();
            _fileService.Save($"{_dialogService.FilePath}\\{Filename}.json", edges);
            _dialogService.ShowMessage("Сохранено в файл!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(ex.Message);
        }
    }

    private void OpenFromFile(object? o)
    {
        try
        {
            if (!_dialogService.OpenFileDialog())
            {
                return;
            }

            var edges = _fileService.Open(_dialogService.FilePath);
            _window.CanvasMain.Children.Clear();
            VertexViewModels.Clear();
            _vertexCounter = 0;
            _edgeCounter = 0;
            var vertexes = new HashSet<Vertex>();
            foreach (var edge in edges)
            {
                vertexes.Add(edge.VertexA);
                vertexes.Add(edge.VertexB);
            }

            foreach (var vertex in vertexes)
            {
                CreateVertexViewModel(vertex.Content, vertex.X, vertex.Y);
            }

            foreach (var edge in edges)
            {
                SelectedVertex1 = VertexViewModels.First(v => v.Text == edge.VertexA.Content);
                SelectedVertex2 = VertexViewModels.First(v => v.Text == edge.VertexB.Content);
                CreateEdgeViewModel();
            }

            SelectedVertex1 = null;
            SelectedVertex2 = null;
            
            _dialogService.ShowMessage("Граф успешно загружен из файла!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(ex.Message);
        }
    }
}

public enum EditMode
{
    Add,
    Remove
}