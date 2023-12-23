namespace AnDS_lab5.Algorithms;

public class Dfs(int v)
{
    private readonly Dictionary<string, List<string>?> _adjList = new(v);
    private readonly List<DfsStep> _steps = new();
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

    private void DfsUtil(string v, IDictionary<string, bool> visited)
    {
        visited[v] = true;
        foreach (string neighbor in _adjList[v]!
                     .Where(neighbor => !visited[neighbor]))
        {
            _path.Add(neighbor);
            _steps.Add(new DfsStep { From = v, To = neighbor, CurrentPath = string.Join(", ", _path)});
            DfsUtil(neighbor, visited);
        }
    }

    public IEnumerable<DfsStep> DfsStart(string startVertex)
    {
        var visited = _adjList.Keys.ToDictionary(v => v, _ => false);
        _steps.Add(new DfsStep { From = "", To = startVertex, CurrentPath = "" });
        _path.Add(startVertex);
        DfsUtil(startVertex, visited);
        return _steps;
    }
}