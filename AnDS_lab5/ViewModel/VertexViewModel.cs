using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace AnDS_lab5.ViewModel;

public sealed class VertexViewModel : INotifyPropertyChanged
{
    private string _text = "";
    private double _x;
    private double _y;
    private TextBox _box = null!;
    private Ellipse _ellipse = null!;

    public List<(EdgeViewModel, int)> Edges { get; set; } = new();

    public TextBox Box
    {
        get => _box;
        set
        {
            _box = value;
            Canvas.SetTop(Box, _x);
            Canvas.SetTop(Box, _y + 30.0);
        }
    }

    public Ellipse Ellipse
    {
        get => _ellipse;
        set
        {
            _ellipse = value;
            Canvas.SetTop(Ellipse, _x + 10.0);
            Canvas.SetTop(Ellipse, _y);
        }
    }
    
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged();
        }
    }

    public double X
    {
        get => _x;
        set
        {
            _x = value;
            Canvas.SetLeft(Ellipse, _x + 10.0);
            Canvas.SetLeft(Box, _x);
            ChangeX();
            OnPropertyChanged();
        }
    }

    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            Canvas.SetTop(Ellipse, _y);
            Canvas.SetTop(Box, _y + 30.0);
            ChangeY();
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ChangeX()
    {
        foreach (var edgePair in Edges)
        {
            if (edgePair.Item1 is CircleEdgeViewModel circleEdgeViewModel)
            {
                circleEdgeViewModel.X = _x;
                continue;
            }
            
            if (edgePair.Item2 == 1)
            {
                edgePair.Item1.X1 = _x;
            }
            else
            {
                edgePair.Item1.X2 = _x;
            }
        }
    }

    private void ChangeY()
    {
        foreach (var edgePair in Edges)
        {
            if (edgePair.Item1 is CircleEdgeViewModel circleEdgeViewModel)
            {
                circleEdgeViewModel.Y = _y;
                continue;
            }
            
            if (edgePair.Item2 == 1)
            {
                edgePair.Item1.Y1 = _y;
            }
            else
            {
                edgePair.Item1.Y2 = _y;
            }
        }
    }

    public override string ToString()
        => Text;

    public override bool Equals(object? obj)
    {
        if (obj is not VertexViewModel other)
        {
            return false;
        }

        return Text.Equals(other.Text) && Math.Abs(X - other.X) < 0.01 && Math.Abs(Y - other.Y) < 0.01;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_text, _x, _y, _box, _ellipse);
    }
}