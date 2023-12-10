namespace AnDS_lab5.Model;

// ReSharper disable once NonReadonlyMemberInGetHashCode
public class Vertex
{
    public string Content { get; init; } = null!;
    public double X { get; init; }
    public double Y { get; init; }
    public double ValueLabel { get; set; } = -1;
    public bool IsChecked { get; set; }
    public Vertex PredVertex { get; set; } = null!;

    public override bool Equals(object? o)
    {
        if (o is not Vertex other) return false;
        return Content.Equals(other.Content)
            && Math.Abs(X - other.X) < 0.000001
            && Math.Abs(Y - other.Y) < 0.000001;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Content, X, Y, ValueLabel, IsChecked, PredVertex);
    }
}