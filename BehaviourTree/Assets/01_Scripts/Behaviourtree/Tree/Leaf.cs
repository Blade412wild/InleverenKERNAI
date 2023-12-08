using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public delegate void PhaseName(string name);
    public static event PhaseName UIUpdate;

    public Leaf()
    {

    }

    public Leaf(string _name, Tick _procesMethod)
    {
        Name = _name;
        ProcessMethod = _procesMethod;
    }

    public override Status Process()
    {
        //Debug.Log(Name);
        UIUpdate?.Invoke(Name);

        if (ProcessMethod != null)
        {
            return ProcessMethod();
        }

        return Status.Failed;
    }
}
