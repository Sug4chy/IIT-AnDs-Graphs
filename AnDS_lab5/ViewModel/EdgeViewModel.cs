﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using AnDS_lab5.Model;

// ReSharper disable ValueParameterNotUsed

namespace AnDS_lab5.ViewModel;

public class EdgeViewModel : INotifyPropertyChanged
{
    private VertexViewModel _vertex1 = null!;
    private VertexViewModel _vertex2 = null!;
    private TextBox _box = null!;
    private int _weight;

    public Shape Line { get; set; } = null!;

    public double Thickness
    {
        get => Line.StrokeThickness;
        set
        {
            Line.StrokeThickness = value;
            OnPropertyChanged();
        }
    }

    public int Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            OnPropertyChanged();
        }
    }
    
    public virtual TextBox Box
    {
        get => _box;
        set
        {
            _box = value;
            Panel.SetZIndex(Box, 1);
            Canvas.SetTop(Box, (Y1 + Y2) / 2 - 20d);
            Canvas.SetLeft(Box, (X1 + X2) / 2 - 20d);
        }
    }
    
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
        set
        {
            Canvas.SetLeft(Box, (X1 + X2) / 2 - 20d);
            OnPropertyChanged(); 
        }
    }

    public double X2
    {
        get => Vertex2.X + 25d;
        set
        {
            Canvas.SetLeft(Box, (X1 + X2) / 2 - 20d);
            OnPropertyChanged();
        }
    }

    public double Y1
    {
        get => Vertex1.Y + 15d;
        set
        {
            Canvas.SetTop(Box, (Y1 + Y2) / 2 - 20d);
            OnPropertyChanged();
        }
    }

    public double Y2
    {
        get => Vertex2.Y + 15d;
        set
        {
            Canvas.SetTop(Box, (Y1 + Y2) / 2 - 20d);
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual Edge ToEdge() 
        => new(_vertex1.ToVertex(), _vertex2.ToVertex(), _weight);
    
}