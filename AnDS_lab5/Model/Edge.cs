namespace AnDS_lab5.Model;

public struct Edge(Vertex vertexA, Vertex vertexB, int weight) : IComparable<Edge>
{
    public int Weight { get; set; } = weight;
    public Vertex VertexA { get; set; } = vertexA;
    public Vertex VertexB { get; set; } = vertexB;
    public int CompareTo(Edge other)
        =>  Weight.CompareTo(other.Weight);

}