using Graphs;
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
            { "dump=", "The path to the dump to be analyzed.", p => analyzeArguments.AppPath = p },
            { "namespace=", "filter the leaks to a specific namespace", n => analyzeArguments.Namespace = n ?? ""},
            {"distinct-only", "only display distinct nodes responsible for leak (make output more digestible)", d => analyzeArguments.DistinctOnly = d is not null},
            { "help|h", "Show this help message", h => ShowHelp = h is not null },
        };
    }

    protected override int InvokeInternal()
    {
        // invoke functionality for Analyze command
        var heapDump = new GCHeapDump(analyzeArguments.AppPath);
        var graph = heapDump.MemoryGraph;
        // with the memory graph what needs to be done?
        // need to iterate through all the nodes of the graph and detect strongly connected cycles...
        Console.WriteLine("Mem Graph Nodes Count: " + graph.NodeCount);
        FindSCC findScc = new();
        findScc.Init(graph, Console.Out, Console.Out, analyzeArguments.DistinctOnly, analyzeArguments.Namespace);
        findScc.FindCycles(graph);

        Console.WriteLine("distinct nodes of interest:");
        foreach (var node in findScc.respNodes)
        {
            Console.WriteLine(node);
        }
        return 0;
    }
}