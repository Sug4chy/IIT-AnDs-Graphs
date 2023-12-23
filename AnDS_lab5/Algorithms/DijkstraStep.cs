using AnDS_lab5.Model;

namespace AnDS_lab5.Algorithms;

public record DijkstraStep
{
    public string? ResultPath;
    public DijkstraStepEnum StepType;
    public Vertex? CheckedVertex;
    public double NewLabel;
    public double OldLabel;
}

public enum DijkstraStepEnum
{
    FindMinPath,
    CheckedVertex,
    SetValueLabel,
    UncheckedVertex,
    LabelsComparison
}