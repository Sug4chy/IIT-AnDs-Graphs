namespace AnDS_lab5.Algorithms;

public class FordFulkerson(int verticesCount)
{
    private bool Bfs(IReadOnlyList<List<int>> graph, int s, int t, IList<int> p)
    {
        bool[] visited = new bool[verticesCount];
        for (int i = 0; i < verticesCount; ++i)
        {
            visited[i] = false;
        }

        var queue = new Queue<int>();
        queue.Enqueue(s);
        visited[s] = true;
        p[s] = -1;

        while (queue.Count != 0)
        {
            int u = queue.Dequeue();
            for (int v = 0; v < verticesCount; v++)
            {
                if (!visited[v] && graph[u][v] > 0)
                {
                    queue.Enqueue(v);
                    p[v] = u;
                    visited[v] = true;
                }
            }
        }

        return visited[t];
    }
    
    public int StartFordFulkerson(List<int>[] graph, int s, int t)
    {
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

        int[] p = new int[verticesCount];
        int maxFlow = 0;

        while (Bfs(graphCopy, s, t, p))
        {
            int pathFlow = int.MaxValue;
            for (v = t; v != s; v = p[v])
            {
                u = p[v];
                pathFlow = Math.Min(pathFlow, graphCopy[u][v]);
            }

            for (v = t; v != s; v = p[v])
            {
                u = p[v];
                graphCopy[u][v] -= pathFlow;
                graphCopy[v][u] += pathFlow;
            }

            maxFlow += pathFlow;
        }

        return maxFlow;
    }
}