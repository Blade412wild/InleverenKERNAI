using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPatrolPoints : MonoBehaviour
{
    public Transform Trans;
    public Collider Col;

    public enum PatrolRoute { Normal, Special}
    public PatrolRoute Route;


    // Start is called before the first frame update
    void Start()
    {
        Trans = transform;
        Col = GetComponent<Collider>();
    }
}
