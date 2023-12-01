using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Shapes;

// ReSharper disable ValueParameterNotUsed

namespace AnDS_lab5.ViewModel;

public class EdgeViewModel : INotifyPropertyChanged
{
    private VertexViewModel _vertex1 = null!;
    private VertexViewModel _vertex2 = null!;

    public Line Line { get; set; } = null!;

    public VertexViewModel Vertex1
    {
        get => _vertex1;
        set
        {
            _vertex1 = value;
            _vertex1.Edges.Add((this, 1));
        }
    }

    public VertexViewModel Vertex2
    {
        get => _vertex2;
        set
        {
            _vertex2 = value;
            _vertex2.Edges.Add((this, 2));
        }
    }

    public double X1
    {
        get => Vertex1.X + 25d;
        set => OnPropertyChanged();
    }

    public double X2
    {
        get => Vertex2.X + 25d;
        set => OnPropertyChanged();
    }

    public double Y1
    {
        get => Vertex1.Y + 15d;
        set => OnPropertyChanged();
    }

    public double Y2
    {
        get => Vertex2.Y + 15d;
        set => OnPropertyChanged();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}