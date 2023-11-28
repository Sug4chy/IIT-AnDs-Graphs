namespace AnDS_lab5.Model;

public record Vertex(string Content)
{
    public string Content { get; set; } = Content;
    public int X { get; set; }
    public int Y { get; set; }
}