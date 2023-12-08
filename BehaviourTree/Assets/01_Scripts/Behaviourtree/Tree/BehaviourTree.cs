using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        Name = "Tree";
    }

    public BehaviourTree(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
        return Childeren[currentChild].Process();
    }

    struct NodeLevel
    {
        public int level;
        public Node node;
    }


    public void PrintTree()
    {
        string treePrintOut = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode });

        while(nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintOut += new string('-', nextNode.level) + nextNode.node.Name + "\n";
            for( int i = nextNode.node.Childeren.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.Childeren[i] });
            }
        }

        Debug.Log(treePrintOut);

    }
}
