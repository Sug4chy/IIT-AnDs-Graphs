﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AnDS_lab5.Algorithms;
using AnDS_lab5.Model;
using AnDS_lab5.Service;
using AnDS_lab5.View;

namespace AnDS_lab5.ViewModel;

public sealed class OrientedRedactorViewModel : INotifyPropertyChanged
{
    private EditMode _mode;
    private readonly OrientedRedactorWindow _window = null!;
    private VertexViewModel? _selectedVertex1;
    private VertexViewModel? _selectedVertex2;
    private int _edgeCounter;
    private int _vertexCounter;
    private Ellipse? _dragObject;
    private Point _offset;
    private readonly JsonFileService _fileService = new();
    private readonly DefaultDialogService _dialogService = new();
    private string _filename = "";
    private readonly List<EdgeViewModel> _edgeViewModels = new();
    private readonly List<EdgeViewModel> _reverseEdges = new();
    private SolidColorBrush _color = Brushes.Black;

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
    public ICommand StartFordFulkersonCommand { get; } = null!;

    public OrientedRedactorViewModel(OrientedRedactorWindow window)
    {
        _window = window;
        AddVertexCommand = new RelayCommand(AddVertex, _ => Mode == EditMode.Add);
        AddEdgeCommand = new RelayCommand(AddEdge, _ => Mode == EditMode.Add);
        SetCreatingModeCommand = new RelayCommand(SetCreatingMode, _ => Mode == EditMode.Remove);
        SetDeletingModeCommand = new RelayCommand(SetDeletingMode, _ => Mode == EditMode.Add);
        SaveToFileCommand = new RelayCommand(SaveToFile);
        OpenFromFileCommand = new RelayCommand(OpenFromFile);
        StartFordFulkersonCommand = new RelayCommand(StartFordFulkerson);
    }

    public OrientedRedactorViewModel() { }
    
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
                        _window.CanvasMain.Children.Remove(edge.Box);
                    }

                    foreach (var vertexViewModel in VertexViewModels)
                    {
                        vertexViewModel.Edges =
                            vertexViewModel.Edges.Where(tuple => !edges.Contains(tuple.Item1)).ToList();
                    }

                    VertexViewModels.Remove(vertex);
                    break;
                }
                case Arrow line:
                {
                    var edge = _edgeViewModels.First(edge => edge.Line.Equals(line));
                    _edgeViewModels.Remove(edge);
                    _window.CanvasMain.Children.Remove(line);
                    foreach (object? obj in _window.CanvasMain.Children)
                    {
                        if (obj is not TextBox box)
                        {
                            continue;
                        }
                        
                        if (Math.Abs(Canvas.GetLeft(box) - (line.X1 + line.X2) / 2) < 0.0000001
                            && Math.Abs(Canvas.GetTop(box) - (line.Y1 + line.Y2) / 2) < 0.0000001)
                        {
                            _window.CanvasMain.Children.Remove(box);
                        }
                    }
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
        ellipse.SetBinding(Shape.FillProperty, new Binding
        {
            Path = new PropertyPath("Color"),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new BrushConverter()
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
        ellipse.SetBinding(Shape.FillProperty, new Binding
        {
            Path = new PropertyPath("Color"),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Converter = new BrushConverter()
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
                    Stroke = _color,
                    DataContext = edgeViewModel
                };
                var box = new TextBox
                {
                    Height = 20,
                    Width = 50,
                    TextAlignment = TextAlignment.Center,
                    DataContext = edgeViewModel,
                };

                edgeViewModel.Vertex = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
                edgeViewModel.Ellipse = edge;
                edgeViewModel.Box = box;

                box.SetBinding(TextBox.TextProperty,
                    new Binding
                    {
                        Path = new PropertyPath("Weight"), Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });

                _edgeViewModels.Add(edgeViewModel);
                
                Panel.SetZIndex(edge, 0);
                _window.CanvasMain.Children.Add(edge);
                _window.CanvasMain.Children.Add(box);
            }
            else
            {
                var edgeViewModel = new EdgeViewModel();
                var edge = new Arrow
                {
                    HeadHeight = 10,
                    HeadWidth = 10,
                    Stroke = _color,
                    DataContext = edgeViewModel
                };
                var box = new TextBox
                {
                    Height = 20,
                    Width = 50,
                    TextAlignment = TextAlignment.Center,
                    DataContext = edgeViewModel,
                };

                edgeViewModel.Vertex1 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
                edgeViewModel.Vertex2 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex2!)];
                edgeViewModel.Line = edge;
                edgeViewModel.Box = box;

                edge.SetBinding(Arrow.X1Property,
                    new Binding
                    {
                        Path = new PropertyPath("X1"), Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                edge.SetBinding(Arrow.X2Property, new Binding
                {
                    Path = new PropertyPath("X2"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                edge.SetBinding(Arrow.Y1Property, new Binding
                {
                    Path = new PropertyPath("Y1"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                edge.SetBinding(Arrow.Y2Property, new Binding
                {
                    Path = new PropertyPath("Y2"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                box.SetBinding(TextBox.TextProperty,
                    new Binding
                    {
                        Path = new PropertyPath("Weight"), Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });

                _edgeViewModels.Add(edgeViewModel);
                
                Panel.SetZIndex(edge, 0);
                _window.CanvasMain.Children.Add(edge);
                _window.CanvasMain.Children.Add(box);
            }

            _window.ComboBoxes.Visibility = Visibility.Hidden;
            _edgeCounter--;

            SelectedVertex1 = null;
            SelectedVertex2 = null;
        }
    }
    
    private void CreateEdgeViewModel(int weight)
    {
        if (SelectedVertex1!.Equals(SelectedVertex2!))
        {
            var edgeViewModel = new CircleEdgeViewModel();
            var edge = new Ellipse
            {
                Height = 50,
                Width = 50,
                Fill = Brushes.Transparent,
                Stroke = _color,
                DataContext = edgeViewModel
            };
            var box = new TextBox
            {
                Height = 20,
                Width = 50,
                TextAlignment = TextAlignment.Center,
                DataContext = edgeViewModel
            };
            edgeViewModel.Vertex = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
            edgeViewModel.Ellipse = edge;
            edgeViewModel.Box = box;
            edgeViewModel.Weight = weight;

            box.SetBinding(TextBox.TextProperty,
                new Binding
                {
                    Path = new PropertyPath("Weight"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            _edgeViewModels.Add(edgeViewModel);
            
            Panel.SetZIndex(edge, 0);
            _window.CanvasMain.Children.Add(edge);
            _window.CanvasMain.Children.Add(box);
        }
        else
        {
            var edgeViewModel = new EdgeViewModel();
            var edge = new Arrow
            {
                HeadHeight = 10,
                HeadWidth = 10,
                Stroke = _color,
                DataContext = edgeViewModel
            };
            var box = new TextBox
            {
                Height = 20,
                Width = 50,
                TextAlignment = TextAlignment.Center,
                DataContext = edgeViewModel
            };

            edgeViewModel.Vertex1 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex1!)];
            edgeViewModel.Vertex2 = VertexViewModels[VertexViewModels.IndexOf(SelectedVertex2!)];
            edgeViewModel.Line = edge;
            edgeViewModel.Box = box;
            edgeViewModel.Weight = weight;

            edge.SetBinding(Arrow.X1Property,
                new Binding
                {
                    Path = new PropertyPath("X1"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            edge.SetBinding(Arrow.X2Property, new Binding
            {
                Path = new PropertyPath("X2"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            edge.SetBinding(Arrow.Y1Property, new Binding
            {
                Path = new PropertyPath("Y1"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            edge.SetBinding(Arrow.Y2Property, new Binding
            {
                Path = new PropertyPath("Y2"), Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            box.SetBinding(TextBox.TextProperty,
                new Binding
                {
                    Path = new PropertyPath("Weight"), Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            _edgeViewModels.Add(edgeViewModel);
            
            Panel.SetZIndex(edge, 0);
            _window.CanvasMain.Children.Add(edge);
            _window.CanvasMain.Children.Add(box);
        }
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
            _edgeViewModels.Clear();
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
                CreateEdgeViewModel(edge.Weight);
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

    private void StartFordFulkerson(object? o)
    {
        var adjList = new List<int>[VertexViewModels.Count];
        int i = 0;
        foreach (var vertexViewModel in VertexViewModels)
        {
            adjList[i] = new List<int>();
            var edges = vertexViewModel.Edges
                .Where(t => t.Item2 == 1)
                .Select(t => t.Item1)
                .ToList();

            foreach (var v in VertexViewModels)
            {
                int weight = edges.FirstOrDefault(e => e.Vertex2.Equals(v))?.Weight ?? 0;
                adjList[i].Add(weight);
            }

            i++;
        }

        var chooseWindow = new FordFulkersonChooseWindow(VertexViewModels);
        int index1 = 0;
        int index2 = VertexViewModels.Count - 1;
        if (chooseWindow.ShowDialog() == true)
        {
            index1 = VertexViewModels.IndexOf(chooseWindow.SelectedItem1 ?? VertexViewModels[0]);
            index2 = VertexViewModels.IndexOf(chooseWindow.SelectedItem2 ?? VertexViewModels[^1]);
        }

        var algorithm = new FordFulkerson(VertexViewModels.Count, 
            VertexViewModels.Select(v => v.Text).ToArray());
        var steps = algorithm.StartFordFulkerson(adjList, index1, index2);
        HandleSteps(steps);
    }

    private async void HandleSteps(List<FordFulkersonStep> steps)
    {
        _reverseEdges.Clear();
        var stepStrings = new ObservableCollection<string>();
        var stepsWindow = new StepsWindow(stepStrings);
        stepsWindow.Show();
        foreach (var step in steps)
        {
            switch (step.StepType)
            {
                case FordFulkersonStepEnum.MaxFlow:
                    stepStrings.Add($"Найден максимальный поток и он равен {step.MaxFlow}");
                    break;
                case FordFulkersonStepEnum.FindNewPath:
                    stepStrings.Add("Нашли новый маршрут между истоком и стоком");
                    LightNewPath(step.NewPath!);
                    break;
                case FordFulkersonStepEnum.MinFlowInNewPath:
                    stepStrings.Add($"Нашли минимальный поток в найденном пути.\nОн равен {step.MinFlowInNewPath}");
                    break;
                case FordFulkersonStepEnum.AddReverseEdge:
                    stepStrings.Add(
                        $"Строим \"обратное\" ребро между вершинами " +
                        $"\"{VertexViewModels[step.ReverseEdgeFromIndex]}\" и \"{VertexViewModels[step.ReverseEdgeToIndex]}\"");
                    AddReverseEdge(step.ReverseEdgeFromIndex, step.ReverseEdgeToIndex, step.ReverseEdgeWeight);
                    break;
                case FordFulkersonStepEnum.AddMinFlowToMaxFlow:
                    stepStrings.Add("Прибалвяем найденный минимальный поток к счётчику максимального потока \nвсей сети.");
                    break;
                case FordFulkersonStepEnum.StartBfs:
                    stepStrings.Add("Начинаем обход графа в ширину для поиска нового пути от истока до стока");
                    break;
                case FordFulkersonStepEnum.EnqueueValue:
                    stepStrings.Add($"Заходим в вершину {VertexViewModels[step.EnqueueVertexWithIndex]},\n добавляем её в очередь и помечаем как пройденную");
                    break;
                default:
                    continue;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        _dialogService.ShowMessage($"Алгоритм закончен!\nМаксимальный поток = {steps[^1].MaxFlow}");
        ResetGraphState();
    }

    private void LightNewPath(IEnumerable<string> path)
    {
        path = path.Reverse();
        var r = new Random();
        _color = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), 
            (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
        VertexViewModel? prevVertex = null;
        foreach (var vertex in path
                     .Select(vertexText => VertexViewModels
                         .First(v => v.Text == vertexText)))
        {
            vertex.Color = _color;
            if (prevVertex is not null)
            {
                var edge = _edgeViewModels
                    .First(e => e.Vertex1.Equals(prevVertex) && e.Vertex2.Equals(vertex));
                edge.Line.Stroke = _color;
            }

            prevVertex = vertex;
        }
    }

    private void AddReverseEdge(int from, int to, int weight)
    {
        SelectedVertex1 = VertexViewModels[from];
        SelectedVertex2 = VertexViewModels[to];
        CreateEdgeViewModel(weight);
        _reverseEdges.Add(_edgeViewModels[^1]);
        SelectedVertex1 = null;
        SelectedVertex2 = null;
    }
    
    private void ResetGraphState()
    {
        foreach (var v in VertexViewModels)
        {
            v.Color = Brushes.Aqua;
        }

        foreach (var e in _edgeViewModels)
        {
            e.Line.Stroke = Brushes.Black;
        }

        foreach (var reverseEdge in _reverseEdges)
        {
            _edgeViewModels.Remove(reverseEdge);
            _window.CanvasMain.Children.Remove(reverseEdge.Box);
            _window.CanvasMain.Children.Remove(reverseEdge.Line);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}