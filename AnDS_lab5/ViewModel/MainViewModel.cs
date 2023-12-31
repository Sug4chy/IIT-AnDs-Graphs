﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    private readonly List<EdgeViewModel> _edgeViewModels = new();
    private readonly List<(TextBlock, VertexViewModel)> _labels = new();
    private readonly List<EdgeViewModel> _ostovEdges = new();

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
    public ICommand StartDfsCommand { get; } = null!;
    public ICommand StartBfsCommand { get; } = null!;
    public ICommand OpenOrientedRedactorCommand { get; } = null!;
    public ICommand StartDijkstraCommand { get; } = null!;
    public ICommand StartPrimCommand { get; } = null!;

    public MainViewModel(MainWindow window)
    {
        _window = window;
        AddVertexCommand = new RelayCommand(AddVertex, _ => Mode == EditMode.Add);
        AddEdgeCommand = new RelayCommand(AddEdge, _ => Mode == EditMode.Add);
        SetCreatingModeCommand = new RelayCommand(SetCreatingMode, _ => Mode == EditMode.Remove);
        SetDeletingModeCommand = new RelayCommand(SetDeletingMode, _ => Mode == EditMode.Add);
        SaveToFileCommand = new RelayCommand(SaveToFile);
        OpenFromFileCommand = new RelayCommand(OpenFromFile);
        StartDfsCommand = new RelayCommand(StartDfs);
        StartBfsCommand = new RelayCommand(StartBfs);
        OpenOrientedRedactorCommand = new RelayCommand(OpenOrientedRedactor);
        StartDijkstraCommand = new RelayCommand(StartDijkstra);
        StartPrimCommand = new RelayCommand(StartPrim);
    }

    public MainViewModel() { }
    
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
                case Line line:
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
                    Stroke = Brushes.Black,
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
                var edge = new Line
                {
                    Stroke = Brushes.Black,
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
                Stroke = Brushes.Black,
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
            var edge = new Line
            {
                Stroke = Brushes.Black,
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
            foreach (var edge in edges)
            {
                if (!VertexViewModels.Any(v => v.ToVertex().Equals(edge.VertexA)))
                {
                    CreateVertexViewModel(edge.VertexA.Content, edge.VertexA.X, edge.VertexA.Y);
                }
                
                if (!VertexViewModels.Any(v => v.ToVertex().Equals(edge.VertexB)))
                {
                    CreateVertexViewModel(edge.VertexB.Content, edge.VertexB.X, edge.VertexB.Y);
                }
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

    private void StartDfs(object? o)
    {
        var vertexChooseWindow = new VertexChooseWindow(VertexViewModels);
        string content = vertexChooseWindow.ShowDialog() == true 
            ? vertexChooseWindow.SelectedVertex.Text 
            : VertexViewModels[0].Text;
        
        var edges = _edgeViewModels
            .Select(GetEdgeVertexes)
            .ToArray();

        var dfs = new Dfs(_edgeViewModels.Count);
        foreach (var edge in edges)
        {
            dfs.AddEdge(edge.Item1, edge.Item2);
        }

        var steps = dfs.DfsStart(content);
        HandleSteps(steps);
    }

    private static (string, string) GetEdgeVertexes(EdgeViewModel e)
        => e is CircleEdgeViewModel circleE
            ? (circleE.Vertex.Text, circleE.Vertex.Text)
            : (e.Vertex1.Text, e.Vertex2.Text);

    private async void HandleSteps(IEnumerable<DfsStep> steps)
    {
        var stepsString = new ObservableCollection<string>();
        var stepsWindow = new StepsWindow(stepsString);
        stepsWindow.Show();
        var dfsSteps = steps as DfsStep[] ?? steps.ToArray();
        foreach (var step in dfsSteps)
        {
            if (step.From == "")
            {
                stepsString.Add($"Начинаем обход с вершины {step.To}");
                var vertex = VertexViewModels.First(v => v.Text == step.To);
                vertex.Color = Brushes.Red;
            }
            else
            {
                stepsString.Add($"Посещаем вершину {step.To} из вершины {step.From}\nМаршрут обхода в данный момент: {step.CurrentPath}");
                var vertex = VertexViewModels.First(v => v.Text == step.To);
                vertex.Color = Brushes.Red;
                var edge = _edgeViewModels.First(e => 
                    (e.Vertex1.Text == step.From && e.Vertex2.Text == step.To) 
                    || (e.Vertex1.Text == step.To && e.Vertex2.Text == step.From));
                edge.Thickness = 2.5;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        stepsString.Add($"Итог обхода: {dfsSteps[^1].CurrentPath}");
        MessageBox.Show("Обход окончен!");
        ResetGraphState();
    }

    private void ResetGraphState()
    {
        foreach (var v in VertexViewModels)
        {
            v.Color = Brushes.Aqua;
        }

        foreach (var e in _edgeViewModels)
        {
            e.Thickness = 1d;
            e.Line.Stroke = Brushes.Black;
        }

        foreach (var label in _labels)
        {
            _window.CanvasMain.Children.Remove(label.Item1);
        }
    }

    private void StartBfs(object? o)
    {
        var vertexChooseWindow = new VertexChooseWindow(VertexViewModels);
        string content = vertexChooseWindow.ShowDialog() == true 
            ? vertexChooseWindow.SelectedVertex.Text 
            : VertexViewModels[0].Text;
        
        var edges = _edgeViewModels
            .Select(GetEdgeVertexes)
            .ToArray();

        var dfs = new Bfs(_edgeViewModels.Count);
        foreach (var edge in edges)
        {
            dfs.AddEdge(edge.Item1, edge.Item2);
        }

        var steps = dfs.BfsStart(content);
        HandleSteps(steps);
    }
    
    private async void HandleSteps(IEnumerable<BfsStep> steps)
    {
        var stepsString = new ObservableCollection<string>();
        var stepsWindow = new StepsWindow(stepsString);
        stepsWindow.Show();
        var bfsSteps = steps as BfsStep[] ?? steps.ToArray();
        foreach (var step in bfsSteps)
        {
            if (step.From == "")
            {
                stepsString.Add($"Начинаем обход с вершины {step.To}");
                var vertex = VertexViewModels.First(v => v.Text == step.To);
                vertex.Color = Brushes.Blue;
            }
            else
            {
                stepsString.Add($"Посещаем вершину {step.To} из вершины {step.From}\nМаршрут обхода в данный момент: {step.CurrentPath}");
                var vertex = VertexViewModels.First(v => v.Text == step.To);
                vertex.Color = Brushes.Blue;
                var edge = _edgeViewModels.First(e => 
                    (e.Vertex1.Text == step.From && e.Vertex2.Text == step.To) 
                    || (e.Vertex1.Text == step.To && e.Vertex2.Text == step.From));
                edge.Thickness = 2.5;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        stepsString.Add($"Итог обхода: {bfsSteps[^1].CurrentPath}");
        MessageBox.Show("Обход окончен!");
        ResetGraphState();
    }

    private void OpenOrientedRedactor(object? o)
        => new OrientedRedactorWindow().Show();

    private void StartDijkstra(object? o)
    {
        var edges = _edgeViewModels.Select(ev => ev.ToEdge());
        var vertices = VertexViewModels.Select(vv => vv.ToVertex()).ToArray();
        var algorithm = new DijkstraAlgorithm(vertices, edges, false);

        var chooseWindow = new DijkstraChooseWindow(VertexViewModels);
        if (chooseWindow.ShowDialog() != true)
        {
            return;
        }

        var begin = vertices[chooseWindow.ComboBox1.SelectedIndex];
        var end = vertices[chooseWindow.ComboBox2.SelectedIndex];
        var steps = algorithm.Run(begin, end);
        HandleSteps(steps);
    }

    private async void HandleSteps(List<DijkstraStep> steps)
    {
        _labels.Clear();
        var stepStrings = new ObservableCollection<string>();
        var stepsWindow = new StepsWindow(stepStrings);
        stepsWindow.Show();
        foreach (var step in steps)
        {
            switch (step.StepType)
            {
                case DijkstraStepEnum.FindMinPath:
                    stepStrings.Add($"Нашли минимальный путь между вершинами: {step.ResultPath}");
                    break;
                case DijkstraStepEnum.CheckedVertex:
                    var a = VertexViewModels
                        .First(v => v.Text == step.CheckedVertex?.Content);
                    a.Color = Brushes.Red;
                    stepStrings.Add($"Отметили вершину {a}, с ней мы больше дел не имеем");
                    break;
                case DijkstraStepEnum.SetValueLabel:
                    SetLabel(step.NewLabel, step.CheckedVertex!);
                    stepStrings.Add($"Устанавливаем \"метку\" {step.NewLabel} на вершину {step.CheckedVertex}, потому что она оказалась меньше предыдущей\n (либо её там до этого не было)");
                    break;
                case DijkstraStepEnum.UncheckedVertex:
                    var b = VertexViewModels
                        .First(v => v.Text == step.CheckedVertex?.Content);
                    b.Color = Brushes.Aqua;
                    stepStrings.Add($"Кратчайший путь до вершины {b} изменился, поэтому нужно будет ещё раз проверить её соседей.");
                    break;
                case DijkstraStepEnum.LabelsComparison:
                    stepStrings.Add($"Сравниваем новую \"метку\" {step.NewLabel} со старой \"меткой\" {step.OldLabel} на вершине {step.CheckedVertex}");
                    break;
                default:
                    continue;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        _dialogService.ShowMessage("Алгоритм завершил свою работу.");
        ResetGraphState();
    }

    private void SetLabel(double label, Vertex v)
    {
        double x = v.X + 17.5;
        double y = v.Y + 5d;
        var (textBlock, vertexViewModel) = _labels
            .FirstOrDefault(tuple => tuple.Item2.Text == v.Content);
        var block = new TextBlock
        {
            Text = label.ToString(CultureInfo.InvariantCulture),
            Height = 15,
            Width = 15
        };
        Canvas.SetTop(block, y);
        Canvas.SetLeft(block, x);
        Panel.SetZIndex(block, 2);
        
        if (vertexViewModel is null)
        {
            _window.CanvasMain.Children.Add(block);
            _labels.Add((block, VertexViewModels.FirstOrDefault(vv => vv.Text == v.Content)!));
        }
        else
        {
            _window.CanvasMain.Children.Remove(textBlock);
            _window.CanvasMain.Children.Add(block);
            _labels.Add((block, vertexViewModel));
        }
    }

    private void StartPrim(object? o)
    {
        _ostovEdges.Clear();
        var chooseWindow = new VertexChooseWindow(VertexViewModels);
        int index = chooseWindow.ShowDialog() == true 
            ? chooseWindow.ComboBox.SelectedIndex 
            : 0;

        var adjList = new List<int>[VertexViewModels.Count];
        for (int i = 0; i < VertexViewModels.Count; i++)
        {
            adjList[i] = new List<int>();
            foreach (var t in VertexViewModels)
            {
                adjList[i].Add(_edgeViewModels
                                   .FirstOrDefault(e => e.Vertex1.Equals(VertexViewModels[i]) 
                                                        && e.Vertex2.Equals(t))?.Weight ?? 0);
            }
        }

        var algorithm = new PrimAlgorithm();
        var steps = algorithm.RunAlgorithm(adjList, index);
        HandleSteps(steps);
    }

    private async void HandleSteps(List<PrimStep> steps)
    {
        var stepStrings = new ObservableCollection<string>();
        var stepsWindow = new StepsWindow(stepStrings);
        stepsWindow.Show();
        foreach (var step in steps)
        {
            switch (step.StepType)
            {
                case PrimStepEnum.AddEdge:
                    stepStrings.Add($"Добавляем в остовное дерево ребро между вершинами {VertexViewModels[step.FromIndex]} и {VertexViewModels[step.ToIndex]}");
                    var b = VertexViewModels[step.FromIndex];
                    var c = VertexViewModels[step.ToIndex];
                    var d = _edgeViewModels.First(e =>
                        e.Vertex1.Equals(b) && e.Vertex2.Equals(c) || e.Vertex1.Equals(c) && e.Vertex2.Equals(b));
                    d.Line.Stroke = Brushes.Red;
                    d.Thickness = 2.5d;
                    c.Color = Brushes.Red;
                    _ostovEdges.Add(d);
                    break;
                case PrimStepEnum.Start:
                    stepStrings.Add($"Начинаем работу алгоритма и сразу добавляем в остовное дерево вершину {VertexViewModels[step.FromIndex]}");
                    var a = VertexViewModels[step.FromIndex];
                    a.Color = Brushes.Red;
                    break;
                case PrimStepEnum.CheckEdge:
                    stepStrings.Add(
                        $"Проверяем рёбра, связанные с вершиной {VertexViewModels[step.FromIndex]}");
                    break;
                default:
                    continue;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        var result = MessageBox.Show("Алгоритм завершил свою работу. Хотите сохранить граф вместе с его остовным деревом?", "Finish", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (result == MessageBoxResult.Yes)
        {
            SaveWithOstovTree();
        }
        
        ResetGraphState();
    }

    private void SaveWithOstovTree()
    {
        SaveToFile(null);
        SaveOstovTree();
    }

    private void SaveOstovTree()
    {
        try
        {
            if (!_dialogService.SaveFileDialog())
            {
                return;
            }

            var edges = _ostovEdges.Select(ev => ev.ToEdge()).ToList();
            _fileService.Save($"{_dialogService.FilePath}\\{Filename}-ostov-tree.json", edges);
            _dialogService.ShowMessage("Сохранено в файл!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(ex.Message);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum EditMode
{
    Add,
    Remove
}