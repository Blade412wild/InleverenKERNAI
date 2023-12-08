using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string _name)
    {
        Name = _name;
    }

    public override Status Process()
    {
        Status childStatus = Childeren[currentChild].Process();
        //Debug.Log("Sequence : " + Childeren[currentChild].Name + "| Status = " + childStatus);

        if (childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Succes)
        {
            currentChild = 0;
            return Status.Succes;
        }

        currentChild++;

        if (currentChild >= Childeren.Count)
        {
            currentChild = 0;
            return Status.Failed;
        }

        return Status.Running;
    }


}
