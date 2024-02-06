using System.Net;

namespace Graphs;

public class FindSCC
{
    private class SCCInfo
    {
        public int m_index;
        public int m_lowLink;
        public Node node; // node is only allocated for the nodes we need to traverse.
        public string type; // We don't expect to have that many nodes so we just store the type here.
    }

    private SCCInfo[] m_sccInfo;
    private Stack<int> m_stack;
    private MemoryGraph m_graph;
    private int index;
    private List<int> m_currentCycle = new();
    private int startNodeIdx;
    public HashSet<string> respNodes = new();
    public string? @namespace;

    public void Init(MemoryGraph graph, TextWriter writer, TextWriter log, string? @namespace = null)
    {
        this.@namespace = @namespace;
        m_graph = graph;
        m_sccInfo = new SCCInfo[(int)graph.NodeIndexLimit];
        for (var i = 0; i < m_sccInfo.Length; i++)
        {
            m_sccInfo[i] = new SCCInfo();
        }
        index = 1;
        m_stack = new Stack<int>();
    }

    private void PrintEdges(int idx)
    {
        var currentNode = m_sccInfo[idx].node;

        for (var childIdx = currentNode.GetFirstChildIndex(); childIdx != NodeIndex.Invalid; childIdx = currentNode.GetNextChildIndex())
        {
            // info about current node
            Console.WriteLine("idx#{0}:{1:x}({2}), child#{3}: {4:x}({5})",
                idx, m_graph.GetAddress((NodeIndex)idx), m_sccInfo[idx].type,
                (int)childIdx, m_graph.GetAddress(childIdx), m_sccInfo[(int)childIdx].type);

            // child node processing
            if (idx == (int)childIdx)
                continue; //full circle has been explored

            if (m_currentCycle.Contains((int)childIdx))
            {
                if ((int)childIdx == startNodeIdx)
                {
                    Console.WriteLine("{0}({1:x}){2}({3:x}) [end of cycle]",
                        m_sccInfo[idx].type,
                        m_graph.GetAddress((NodeIndex)idx),
                        m_sccInfo[(int)childIdx].type,
                        m_graph.GetAddress(childIdx));
                    continue;
                }
                if (m_sccInfo[(int)childIdx].m_index == 1)
                {
                    m_sccInfo[(int)childIdx].m_index = 0;
                    Console.WriteLine("{0}({1:x})",
                        m_sccInfo[idx].type,
                        m_graph.GetAddress((NodeIndex)idx));
                    PrintEdges((int)childIdx);
                }
                else
                {
                    Console.WriteLine("{0}({1:x}){2}({3:x}) [connecting to an existing graph in cycle]",
                        m_sccInfo[idx].type,
                        m_graph.GetAddress((NodeIndex)idx),
                        m_sccInfo[(int)childIdx].type,
                        m_graph.GetAddress(childIdx));
                }
            }
        }
    }

    private void FindCyclesOne(int idx)
    {
        m_sccInfo[idx].m_index = index;
        m_sccInfo[idx].m_lowLink = index;
        index++;
        m_stack.Push(idx);

        m_sccInfo[idx].node = m_graph.AllocNodeStorage();
        var currentNode = m_sccInfo[idx].node;
        m_graph.GetNode((NodeIndex)idx, currentNode);

        for (var childIdx = currentNode.GetFirstChildIndex(); childIdx != NodeIndex.Invalid; childIdx = currentNode.GetNextChildIndex())
        {
            if (m_sccInfo[(int)childIdx].m_index == 0)
            {
                FindCyclesOne((int)childIdx);
                m_sccInfo[idx].m_lowLink = Math.Min(m_sccInfo[idx].m_lowLink, m_sccInfo[(int)childIdx].m_lowLink);
            }
            else if (m_stack.Contains((int)childIdx))
            {
                m_sccInfo[idx].m_lowLink = Math.Min(m_sccInfo[idx].m_lowLink, m_sccInfo[(int)childIdx].m_index);
            }
        }

        if (m_sccInfo[idx].m_index == m_sccInfo[idx].m_lowLink)
        {
            int currentIdx;
            m_currentCycle.Clear();
            var type = m_graph.AllocTypeNodeStorage();
            do
            {
                currentIdx = m_stack.Pop();
                var node = m_sccInfo[currentIdx].node;
                m_graph.GetType(node.TypeIndex, type);
                m_currentCycle.Add(currentIdx);
            } while (idx != currentIdx);

            if (m_currentCycle.Count > 1)
            {
                var nodeName = m_graph.GetType(m_sccInfo[m_currentCycle[0]].node.TypeIndex, type).Name;
                if (!nodeName.Contains(@namespace))
                    return;
                Console.WriteLine("cycle detected");
                Console.WriteLine("node responsible: " + nodeName);
                Console.WriteLine("current cycle length: " + m_currentCycle.Count);
                respNodes.Add(m_graph.GetType(m_sccInfo[m_currentCycle[0]].node.TypeIndex, type).Name);
                // Now print out all the nodes in this cycle.
                List<string> typeNames = new();
                for (var i = m_currentCycle.Count - 1; i >= 0; i--)
                {
                    var nodeInCycle = m_sccInfo[m_currentCycle[i]].node;
                    // Resetting this for printing purpose below.
                    m_sccInfo[m_currentCycle[i]].m_index = 1;
                    m_sccInfo[m_currentCycle[i]].type = m_graph.GetType(nodeInCycle.TypeIndex, type).Name;

                    if (typeNames.Contains(m_sccInfo[m_currentCycle[i]].type))
                        continue;

                    typeNames.Add(m_sccInfo[m_currentCycle[i]].type);
                    Console.WriteLine(m_sccInfo[m_currentCycle[i]].type);
                }
            }
        }
    }

    public void FindCycles(MemoryGraph graph) => FindCyclesOne((int)graph.RootIndex);
}