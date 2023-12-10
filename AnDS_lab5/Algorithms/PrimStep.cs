namespace AnDS_lab5.Algorithms;

public record PrimStep
{
    public PrimStepEnum StepType;
    public int FromIndex;
    public int ToIndex;
}

public enum PrimStepEnum
{
    AddEdge,
    Start,
    CheckEdge
}