namespace AnDS_lab5.Algorithms;

public class Bfs(int v)
{
    private readonly Dictionary<string, List<string>?> _adjList = new(v);
    private readonly List<BfsStep> _steps = new();
    private readonly List<string> _path = new();

    public void AddEdge(string from, string to)
    {
        if (!_adjList.TryGetValue(from, out var value1))
        {
            value1 = new List<string>();
            _adjList.Add(from, value1);
        }

        if (!_adjList.TryGetValue(to, out var value2))
        {
            value2 = new List<string>();
            _adjList.Add(to, value2);
        }

        value1!.Add(to);
        value2!.Add(from);
    }

    private void BfsUtil(string v, IDictionary<string, bool> visited)
    {
        bool[] flags = _adjList[v]!
            .Where(n => !visited[n])
            .Select(n => visited[n])
            .ToArray();
        if (!flags.Contains(false)) return;

        visited[v] = true;
        foreach (string neighbour in _adjList[v]!.Where(neighbour => !visited[neighbour]))
        {
            _path.Add(neighbour);
            _steps.Add(new BfsStep { From = v, To = neighbour, CurrentPath = string.Join(", ", _path)});
            visited[neighbour] = true;
        }

        foreach (string neighbour in _adjList[v]!)
        {
            BfsUtil(neighbour, visited);
        }
    }

    public IEnumerable<BfsStep> BfsStart(string startVertex)
    {
        var visited = _adjList.Keys.ToDictionary(v => v, _ => false);
        _steps.Add(new BfsStep { From = "", To = startVertex, CurrentPath = "" });
        _path.Add(startVertex);
        BfsUtil(startVertex, visited);
        return _steps;
    }
}