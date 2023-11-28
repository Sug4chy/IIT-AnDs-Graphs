namespace AnDS_lab5.Model;

public class Edge(string vertexA, string vertexB, int weight) : IComparable<Edge>
{
    public int Weight { get; set; } = weight;
    public string VertexA { get; set; } = vertexA;
    public string VertexB { get; set; } = vertexB;

    public int CompareTo(Edge? other)
        => other is null ? 1 : Weight.CompareTo(other.Weight);

}