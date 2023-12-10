using AnDS_lab5.Model;

namespace AnDS_lab5.Algorithms;

public class PrimAlgorithm
{
    private readonly List<PrimStep> _steps = new();
    
    public List<PrimStep> RunAlgorithm(List<int>[] graph, int chosenIndex)
    {
        _steps.Clear();
        int verticesCount = graph.Length;
        var spanningTree = new List<int>[verticesCount];
        for (int i = 0; i < verticesCount; i++)
        {
            spanningTree[i] = new List<int>();
            for (int j = 0; j < verticesCount; j++)
            {
                spanningTree[i].Add(0);
            }
        }

        var visited = new HashSet<int>();
        var unvisited = new HashSet<int>();
        var treeEdges = new List<Edge>();

        for (int v = 0; v < verticesCount; v++)
        {
            unvisited.Add(v);
        }

        _steps.Add(new PrimStep { StepType = PrimStepEnum.Start, FromIndex = chosenIndex });
        visited.Add(chosenIndex);
        unvisited.Remove(chosenIndex);

        while (unvisited.Count != 0)
        {
            var edge = new Edge
            {
                Weight = int.MaxValue
            };

            foreach (int from in visited)
            {
                _steps.Add(new PrimStep { StepType = PrimStepEnum.CheckEdge, FromIndex = from });
                for (int to = 0; to < verticesCount; to++)
                {
                    bool isUnvisitedVertex = !visited.Contains(to);
                    bool edgeExists = graph[from][to] != 0 || graph[to][from] != 0;
                    if (edgeExists && isUnvisitedVertex)
                    {
                        int existedWeight = graph[from][to] == 0
                            ? graph[to][from]
                            : graph[from][to];
                        bool edgeIsShorter = edge.Weight > existedWeight;
                        if (edgeIsShorter)
                        {
                            edge = new Edge(new Vertex { Content = from.ToString() },
                                new Vertex { Content = to.ToString() }, existedWeight);
                        }
                    }
                }
            }

            if (edge.Weight != int.MaxValue)
            {
                _steps.Add(new PrimStep
                {
                    StepType = PrimStepEnum.AddEdge,
                    FromIndex = int.Parse(edge.VertexA.Content),
                    ToIndex = int.Parse(edge.VertexB.Content)
                });
                treeEdges.Add(edge);
                visited.Add(int.Parse(edge.VertexB.Content));
                unvisited.Remove(int.Parse(edge.VertexB.Content));
            }
            else
            {
                break;
            }
        }

        foreach (var edge in treeEdges)
        {
            spanningTree[int.Parse(edge.VertexA.Content)][int.Parse(edge.VertexB.Content)] = edge.Weight;
            spanningTree[int.Parse(edge.VertexB.Content)][int.Parse(edge.VertexA.Content)] = edge.Weight;
        }

        return _steps;
    }
}