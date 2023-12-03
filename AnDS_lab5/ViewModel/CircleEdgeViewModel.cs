// ReSharper disable ValueParameterNotUsed

using System.Windows.Controls;
using System.Windows.Shapes;
using AnDS_lab5.Model;

namespace AnDS_lab5.ViewModel;

public class CircleEdgeViewModel : EdgeViewModel
{
    private VertexViewModel _vertex = null!;
    private Ellipse _ellipse = null!;
    private TextBox _box = null!;

    public Ellipse Ellipse
    {
        get => _ellipse;
        set
        {
            _ellipse = value;
            Canvas.SetLeft(Ellipse, X);
            Canvas.SetTop(Ellipse, Y);
        }
    }

    public override TextBox Box
    {
        get => _box;
        set
        {
            _box = value;
            Canvas.SetTop(Box, Y - 20d);
            Canvas.SetLeft(Box, X - 20d);
        }
    }

    public VertexViewModel Vertex
    {
        get => _vertex;
        set
        {
            _vertex = value;
            _vertex.Edges.Add((this, 1));
        }
    }

    public double X
    {
        get => _vertex.X - 20d;
        set
        {
            Canvas.SetLeft(Ellipse, X); 
            Canvas.SetLeft(Box, X - 10d);
        }
    }

    public double Y
    {
        get => _vertex.Y - 20d;
        set
        {
            Canvas.SetTop(Ellipse, Y);
            Canvas.SetTop(Box, Y - 10d);
        }
    }

    public override Edge ToEdge()
        => new(_vertex.ToVertex(), _vertex.ToVertex(), Weight);
}