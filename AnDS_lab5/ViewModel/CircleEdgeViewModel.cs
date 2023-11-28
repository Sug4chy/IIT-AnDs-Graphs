// ReSharper disable ValueParameterNotUsed

using System.Windows.Controls;
using System.Windows.Shapes;

namespace AnDS_lab5.ViewModel;

public class CircleEdgeViewModel : EdgeViewModel
{
    private VertexViewModel _vertex = null!;
    private Ellipse _ellipse = null!;

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
        set => Canvas.SetLeft(_ellipse, X);
    }

    public double Y
    {
        get => _vertex.Y - 20d;
        set => Canvas.SetTop(_ellipse, Y);
    }
}