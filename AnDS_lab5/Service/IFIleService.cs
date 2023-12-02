using AnDS_lab5.Model;

namespace AnDS_lab5.Service;

public interface IFIleService
{
    List<Edge> Open(string filename);
    void Save(string filename, List<Edge> edges);
}