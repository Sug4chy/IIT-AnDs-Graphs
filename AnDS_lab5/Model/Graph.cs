using System.Collections;
using System.IO;
using System.Text;

namespace AnDS_lab5.Model;

public class Graph : IEnumerable<Edge>
{
    private readonly List<Edge> _edges;

    public Graph()
    {
        _edges = new List<Edge>();
    }

    public Graph(Edge edge)
    {
        _edges = new List<Edge>(new[] { edge });
    }

    public IEnumerator<Edge> GetEnumerator()
        => _edges.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(Edge edge)
        => _edges.Add(edge);

    public void Add(Graph graph)
    {
        foreach (var edge in graph)
        {
            _edges.Add(edge);
        }
    }

    public int GetWeight() 
        => _edges.Sum(edge => edge.Weight);

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var edge in _edges)
        {
            sb.AppendLine($"{edge.VertexA} {edge.VertexB} {edge.Weight}");
        }

        return sb.ToString();
    }

    public void Sort()
        => _edges.Sort();

    public void SaveToFile(string fileName)
    {
        File.WriteAllLines($"../../../{fileName}.graph", ToString().Split('\n'));
    }
}