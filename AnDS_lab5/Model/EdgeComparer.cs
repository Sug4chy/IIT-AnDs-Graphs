namespace AnDS_lab5.Model;

public class EdgeComparer : IEqualityComparer<Edge>
{
    public bool Equals(Edge x, Edge y)
    {
        if (x.GetType() != y.GetType()) return false;
        return x.Weight == y.Weight && x.VertexA.Equals(y.VertexA) && x.VertexB.Equals(y.VertexB);
    }

    public int GetHashCode(Edge obj)
    {
        return HashCode.Combine(obj.Weight, obj.VertexA, obj.VertexB);
    }
}