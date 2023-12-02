namespace AnDS_lab5.Model;

public record Vertex(string Content)
{
    public string Content { get; set; } = Content;
    public double X { get; set; }
    public double Y { get; set; }
}