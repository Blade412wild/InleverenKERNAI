using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status{Succes, Running, Failed}
    public Status status;
    public List<Node> Childeren = new List<Node>(); 
    public int currentChild = 0;
    public string Name;

    public Node()
    {

    }

    public Node(string _name)
    {
        Name = _name;
    }

    public virtual Status Process()
    {
        return Childeren[currentChild].Process();
    }



    public void AddChild(Node _child)
    {
        Childeren.Add(_child);
    }

}
