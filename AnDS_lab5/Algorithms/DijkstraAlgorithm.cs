using AnDS_lab5.Model;

namespace AnDS_lab5.Algorithms;

public class DijkstraAlgorithm(IReadOnlyList<Vertex> vertices, IEnumerable<Edge> edges, bool isOriented)
{
    private Vertex? _beginPoint;
    private readonly List<DijkstraStep> _steps = new();

    public List<DijkstraStep> Run(Vertex begin, Vertex end)
    {
        _beginPoint = begin;
        _steps.Clear();
        _beginPoint.ValueLabel = 0;
        OneStep(begin);
        var vertex = begin;
        while (true)
        {
            vertex = GetAnotherUncheckedVertex(vertex);
            if (vertex is null)
            {
                break;
            }

            OneStep(vertex);
        }

        _steps.Add(new DijkstraStep
        {
            ResultPath = string.Join(' ', GetMinPath(end).Select(v => v.Content)),
            StepType = DijkstraStepEnum.FindMinPath
        });
        return _steps;
    }

    private void OneStep(Vertex v)
    {
        foreach (var nextVertex in Pred(v))
        {
            var nv = vertices.First(vertex => vertex.Content == nextVertex.Content);
            double newLabel = v.ValueLabel +
                              (isOriented 
                                  ? GetEdgeOriented(nv, v).Weight 
                                  : GetEdge(nv, v).Weight);
            if (nv.ValueLabel < newLabel && nv.ValueLabel > -1)
            {
                continue;
            }

            nv.ValueLabel = newLabel;
            nv.PredVertex = v;
            _steps.Add(new DijkstraStep
            {
                StepType = DijkstraStepEnum.SetValueLabel,
                NewLabel = newLabel,
                CheckedVertex = nv
            });

            if (!nv.IsChecked)
            {
                continue;
            }

            nv.IsChecked = false;
            _steps.Add(new DijkstraStep
            {
                StepType = DijkstraStepEnum.UncheckedVertex,
                CheckedVertex = nv
            });
        }

        v.IsChecked = true;
        _steps.Add(new DijkstraStep
        {
            CheckedVertex = v,
            StepType = DijkstraStepEnum.CheckedVertex
        });
    }

    private List<Vertex> Pred(Vertex v)
    {
        var firstPoints = edges
            .Where(e => e.VertexA.Equals(v))
            .Select(e => e.VertexB);
        var secondPoints = edges
            .Where(e => e.VertexB.Equals(v))
            .Select(e => e.VertexA);
        var tempResult = firstPoints.ToList();
        foreach (var vertex in secondPoints)
        {
            var fOd = tempResult.FirstOrDefault(v1 => v1.Content == vertex.Content);
            if (fOd is null)
            {
                tempResult.Add(vertex);
            }
        }

        var result = new Vertex[tempResult.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = vertices[Array.IndexOf(tempResult.ToArray(), tempResult[i])];
        }

        return tempResult;
    }

    private Edge GetEdge(Vertex a, Vertex b)
        => edges
            .First(e => e.VertexA.Equals(a) && e.VertexB.Equals(b)
                        || e.VertexB.Equals(a) && e.VertexA.Equals(b));

    private Edge GetEdgeOriented(Vertex a, Vertex b)
        => edges
            .First(e => e.VertexA.Equals(a) && e.VertexB.Equals(b));

    private Vertex? GetAnotherUncheckedVertex(Vertex v)
    {
        var uncheckedPoints = Pred(v)
            .Select(nextVertex => vertices.First(vertex => vertex.Content == nextVertex.Content))
            .Where(nextVertex => !nextVertex.IsChecked)
            .ToArray();
        if (uncheckedPoints.Length == 0)
        {
            return null;
        }

        var minVertex = uncheckedPoints.First();
        double minValue = minVertex.ValueLabel;
        foreach (var vertex in uncheckedPoints)
        {
            if (vertex.ValueLabel >= minValue || vertex.ValueLabel < 0)
            {
                continue;
            }

            minValue = vertex.ValueLabel;
            minVertex = vertex;
        }

        return minVertex;
    }

    private IEnumerable<Vertex> GetMinPath(Vertex end)
    {
        var listOfPoints = new List<Vertex>();
        var temp = end;
        while (temp != _beginPoint)
        {
            listOfPoints.Add(temp);
            temp = temp.PredVertex;
        }

        listOfPoints.Add(_beginPoint);
        listOfPoints.Reverse();
        return listOfPoints;
    }
}