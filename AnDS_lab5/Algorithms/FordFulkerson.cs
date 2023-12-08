namespace AnDS_lab5.Algorithms;

public class FordFulkerson(int verticesCount, IReadOnlyList<string> verticesNames)
{
    private readonly List<FordFulkersonStep> _steps = new();
    
    private bool Bfs(IReadOnlyList<List<int>> graph, int source, int sink, IList<int> parents)
    {
        _steps.Add(new FordFulkersonStep
        {
            StepType = FordFulkersonStepEnum.StartBfs
        });
        bool[] visited = new bool[verticesCount];

        var queue = new Queue<int>();
        queue.Enqueue(source);
        _steps.Add(new FordFulkersonStep
        {
            EnqueueVertexWithIndex = source,
            StepType = FordFulkersonStepEnum.EnqueueValue
        });
        visited[source] = true;
        parents[source] = -1;

        while (queue.Count != 0)
        {
            int u = queue.Dequeue();
            for (int v = 0; v < verticesCount; v++)
            {
                if (visited[v] || graph[u][v] <= 0)
                {
                    continue;
                }

                queue.Enqueue(v);
                _steps.Add(new FordFulkersonStep
                {
                    EnqueueVertexWithIndex = v,
                    StepType = FordFulkersonStepEnum.EnqueueValue
                });
                parents[v] = u;
                visited[v] = true;
            }
        }

        return visited[sink];
    }
    
    public List<FordFulkersonStep> StartFordFulkerson(
        List<int>[] graph, int source, int sink)
    {
        _steps.Clear();
        
        int u, v;
        var graphCopy = new List<int>[verticesCount];
        for (u = 0; u < verticesCount; u++)
        {
            graphCopy[u] = new List<int>(verticesCount);
            for (v = 0; v < verticesCount; v++)
            {
                graphCopy[u].Add(graph[u][v]);
            }
        }

        int[] parents = new int[verticesCount];
        int maxFlow = 0;

        while (Bfs(graphCopy, source, sink, parents))
        {
            var currentPath = new List<string>
            {
                verticesNames[sink]
            };
            int pathFlow = int.MaxValue;
            for (v = sink; v != source; v = parents[v])
            {
                u = parents[v];
                currentPath.Add(verticesNames[u]);
                pathFlow = Math.Min(pathFlow, graphCopy[u][v]);
            }

            _steps.Add(new FordFulkersonStep
            {
                NewPath = currentPath, 
                StepType = FordFulkersonStepEnum.FindNewPath
            });
            _steps.Add(new FordFulkersonStep
            {
                MinFlowInNewPath = pathFlow,
                StepType = FordFulkersonStepEnum.MinFlowInNewPath
            });

            for (v = sink; v != source; v = parents[v])
            {
                u = parents[v];
                graphCopy[u][v] -= pathFlow;
                graphCopy[v][u] += pathFlow;
                _steps.Add(new FordFulkersonStep
                {
                    ReverseEdgeFromIndex = v,
                    ReverseEdgeToIndex = u,
                    ReverseEdgeWeight = pathFlow,
                    StepType = FordFulkersonStepEnum.AddReverseEdge
                });
            }

            _steps.Add(new FordFulkersonStep
            {
                AddToMaxFlowValue = pathFlow,
                StepType = FordFulkersonStepEnum.AddMinFlowToMaxFlow
            });
            maxFlow += pathFlow;
        }

        _steps.Add(new FordFulkersonStep
        {
            MaxFlow = maxFlow, 
            StepType = FordFulkersonStepEnum.MaxFlow
        });

        return _steps;
    }
}