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
    // private TextWriter m_htmlRaw;
    // private TextWriter m_log;
    private int index;
    private List<int> m_currentCycle = new List<int>();
    private int startNodeIdx;
    
    public void Init(MemoryGraph graph, TextWriter writer, TextWriter log)
    {
        m_graph = graph;
        m_sccInfo = new SCCInfo[(int)graph.NodeIndexLimit];
        for (int i = 0; i < m_sccInfo.Length; i++)
        {
            m_sccInfo[i] = new SCCInfo();
        }
        index = 1;
        m_stack = new Stack<int>();
        // m_htmlRaw = writer;
        // m_log = log;
    }
    
    private void FindCyclesOne(int idx)
    {
        m_sccInfo[idx].m_index = index;
        m_sccInfo[idx].m_lowLink = index;
        index++;
        m_stack.Push(idx);

        m_sccInfo[idx].node = m_graph.AllocNodeStorage();
        Node currentNode = m_sccInfo[idx].node;
        m_graph.GetNode((NodeIndex)idx, currentNode);

        for (NodeIndex childIdx = currentNode.GetFirstChildIndex(); childIdx != NodeIndex.Invalid; childIdx = currentNode.GetNextChildIndex())
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
            bool hasCCW = false;
            bool hasRCW = false;
            int currentIdx;
            m_currentCycle.Clear();
            NodeType type = m_graph.AllocTypeNodeStorage();
            do
            {
                currentIdx = m_stack.Pop();

                Node node = m_sccInfo[currentIdx].node;
                m_graph.GetType(node.TypeIndex, type);

                // if (type.Name.StartsWith("[RCW"))
                // {
                //     hasRCW = true;
                // }
                // else if (type.Name.StartsWith("[CCW"))
                // {
                //     hasCCW = true;
                // }

                m_currentCycle.Add(currentIdx);
            } while (idx != currentIdx);

            if (m_currentCycle.Count > 1)
            {
                Console.WriteLine("cycle detected");
                Console.WriteLine("node responsible for cycle: " + m_graph.GetType(m_sccInfo[m_currentCycle[0]].node.TypeIndex, type).Name);
                // m_log.WriteLine("found a cycle of {0} nodes", m_currentCycle.Count);
                // if (hasCCW && hasRCW)
                // {
                    // m_htmlRaw.WriteLine("<font size=\"3\" color=\"blue\">Cycle of {0} nodes<br></font>",
                        // m_currentCycle.Count);
                    // Now print out all the nodes in this cycle.
                    for (int i = m_currentCycle.Count - 1; i >= 0; i--)
                    {
                        Node nodeInCycle = m_sccInfo[m_currentCycle[i]].node;
                        // Resetting this for printing purpose below.
                        m_sccInfo[m_currentCycle[i]].m_index = 1;
                        // string typeName = GetPrintableString(m_graph.GetType(nodeInCycle.TypeIndex, type).Name);
                        // m_sccInfo[m_currentCycle[i]].type = typeName;
                        m_sccInfo[m_currentCycle[i]].type = m_graph.GetType(nodeInCycle.TypeIndex, type).Name;
                        // m_htmlRaw.WriteLine("{0}<br>", typeName);
                    }
                    // m_htmlRaw.WriteLine("<br><br>");

                    // Now print out the actual edges. Reusing the m_index field in SCCInfo.
                    // It doesn't matter where we start, just start from the first one.
                    startNodeIdx = m_currentCycle[m_currentCycle.Count - 1];
                    // m_htmlRaw.WriteLine("<font size=\"3\" color=\"blue\">Paths</font><br>");
                    // PrintEdges(startNodeIdx);
                // }
            }
        }
    }

    public void FindCycles(MemoryGraph graph)
    {
        for(int i =0; i < graph.NodeCount; i++)
        {
            if (m_sccInfo[i].m_index == 0)
            {
                FindCyclesOne(i);
            }
        }
    }
    
}