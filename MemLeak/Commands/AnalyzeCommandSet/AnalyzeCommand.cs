using Mono.Options;

namespace MemLeak.Commands.AnalyzeCommandSet;

internal class AnalyzeCommand : MLCommand
{
    public AnalyzeCommandArguments analyzeArguments = new();

    public AnalyzeCommand() : base("analyze", "Identify strongly connected components in app's heap")
    {
        Options = new OptionSet()
        {
            "usage: analyze [OPTIONS] <pathToApp>",
            "",
            "Identifies culprit objects that are responsible for the strongly connected cycles (which leads to a memory leak) ",
            { "appPath=", "The path to app to be analyzed.", p => analyzeArguments.AppPath = p },
            { "help|h", "Show this help message", h => ShowHelp = h is not null },
        };
    }

    protected override int InvokeInternal()
    {
        // invoke functionality for Analyze command
        var heapDump = new GCHeapDump(analyzeArguments.AppPath);
        var memoryGraph = heapDump.MemoryGraph;
        Console.WriteLine("Mem Graph Nodes Count: " + memoryGraph.NodeCount);
        return 0;
    }
}