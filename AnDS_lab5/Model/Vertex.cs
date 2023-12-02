namespace AnDS_lab5.Model;

public record Vertex(string Content)
{
    public string Content { get; } = Content;
    public double X { get; init; }
    public double Y { get; init; }

#pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public virtual bool Equals(Vertex? other)
#pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    {
        return other is not null 
               && Content.Equals(other.Content) 
               && Math.Abs(X - other.X) < 0.000001 
               && Math.Abs(Y - other.Y) < 0.000001;
    }
}