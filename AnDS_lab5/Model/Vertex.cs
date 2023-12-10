namespace AnDS_lab5.Model;

public class Vertex
{
    public string Content { get; init; } = null!;
    public double X { get; init; }
    public double Y { get; init; }
    public double ValueLabel { get; set; } = -1;
    public bool IsChecked { get; set; }
    public Vertex PredVertex { get; set; } = null!;

    public bool Equals(Vertex other) 
        => Content.Equals(other.Content) 
           && Math.Abs(X - other.X) < 0.000001 
           && Math.Abs(Y - other.Y) < 0.000001;
}