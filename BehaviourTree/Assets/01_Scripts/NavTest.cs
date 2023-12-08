using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    [SerializeField] private Transform movePositionTrans;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(movePositionTrans.position);
    }

    // Update is called once per frame
    void Update()
    {
        //navMeshAgent.destination = movePositionTrans.position;
    }
}
