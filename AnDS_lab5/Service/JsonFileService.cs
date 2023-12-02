using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using AnDS_lab5.Model;

namespace AnDS_lab5.Service;

public class JsonFileService : IFIleService
{
    public List<Edge> Open(string filename)
    {
        var jsonFormatter = new DataContractJsonSerializer(typeof(List<Edge>));
        using var fs = new FileStream(filename, FileMode.OpenOrCreate);
        return (jsonFormatter.ReadObject(fs) as List<Edge>)!;
    }

    public async void Save(string filename, List<Edge> edges)
    {
        await using var fs = new FileStream(filename, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, edges);
    }
}