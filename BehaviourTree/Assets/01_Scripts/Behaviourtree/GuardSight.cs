using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSight : MonoBehaviour
{
    public delegate void Guard(Vector3 lastSeenPos);
    public static event Guard OnPlayerSeen;
    public static event Guard OnPlayerNotSeen;

    public GameObject player;
    public GameObject Eyes;
    public GameObject PlayerMid;

    public Collider PlayerCollider;

    public bool PlayerInSight;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);

    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {

        Transform target = PlayerMid.transform;
        Vector3 dirToTarget = (target.position - Eyes.transform.position).normalized;
        if (Vector3.Angle(Eyes.transform.forward, dirToTarget) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(Eyes.transform.position, target.position);
            Debug.Log(" ------------- Distance to target = " + dstToTarget);


            //Debug.DrawRay(transform.position, dirToTarget, Color.red, dstToTarget);
            //if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            //{
            //    Debug.DrawRay(transform.position, dirToTarget,Color.green, dstToTarget);
            //    PlayerInSight = true;
            //}
            //else
            //{
            //    //Debug.DrawRay(transform.position, dirToTarget, Color.blue, dstToTarget);
            //    //PlayerInSight = false;
            ////}
            //RaycastHit hit;
            //// Does the ray intersect any objects excluding the player layer
            //if (Physics.Raycast(transform.position, dirToTarget, out hit, viewRadius))
            //{

            //    Debug.DrawRay(transform.position, dirToTarget * hit.distance, Color.yellow);
            //    Debug.Log("Did Hit");
            //    PlayerInSight = true;
            //}
            //else
            //{
            //    Debug.DrawRay(transform.position, dirToTarget * 1000, Color.white);
            //    Debug.Log("Did not Hit");
            //    PlayerInSight = false;
            //}


        }

    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInSight == true)
        {
            OnPlayerSeen?.Invoke(player.transform.position);
        }
        else
        {
            OnPlayerNotSeen?.Invoke(player.transform.position);
        }
    }

    private void FixedUpdate()
    {
        Transform target = PlayerMid.transform;
        Vector3 dirToTarget = (target.position - Eyes.transform.position).normalized;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Eyes.transform.position, dirToTarget, out hit, viewRadius) && PlayerCollider == hit.collider)
        {
            PlayerInSight = true;
        }
        else
        {
            PlayerInSight = false;
        }
    }
}
