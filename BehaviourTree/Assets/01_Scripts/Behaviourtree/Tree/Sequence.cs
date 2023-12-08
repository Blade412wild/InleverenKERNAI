using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string _name)
    {
        Name = _name;
    }

    public override Status Process()
    {
        Status childStatus = Childeren[currentChild].Process();
        //Debug.Log("Sequence : " + Childeren[currentChild].Name + "| Status = " + childStatus);

        if(childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Failed)
        {
            return childStatus;
        }

        currentChild++;
        if(currentChild >= Childeren.Count)
        {
            currentChild = 0;
            return Status.Succes;
        }

        return Status.Running;
    }
}
