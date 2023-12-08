using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPatrolPoints : MonoBehaviour
{
    public Transform Trans;
    public Collider Col;


    // Start is called before the first frame update
    void Start()
    {
        Trans = transform;
        Col = GetComponent<Collider>();
    }
}
