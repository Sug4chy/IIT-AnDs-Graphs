namespace AnDS_lab5.Model;

public readonly struct Vertex()
{
    public string Content { get; init; } = null!;
    public double X { get; init; } = 0;
    public double Y { get; init; } = 0;

    public bool Equals(Vertex other) 
        => Content.Equals(other.Content) 
           && Math.Abs(X - other.X) < 0.000001 
           && Math.Abs(Y - other.Y) < 0.000001;
}