namespace AnDS_lab5.Algorithms;

public record FordFulkersonStep
{
    public int MaxFlow;
    public FordFulkersonStepEnum StepType;
    public List<string>? NewPath;
    public int MinFlowInNewPath;
    public int ReverseEdgeFromIndex;
    public int ReverseEdgeToIndex;
    public int ReverseEdgeWeight;
    public int AddToMaxFlowValue;
    public int EnqueueVertexWithIndex;
}

public enum FordFulkersonStepEnum
{
    MaxFlow,
    FindNewPath,
    MinFlowInNewPath,
    AddReverseEdge,
    AddMinFlowToMaxFlow,
    StartBfs,
    EnqueueValue
}