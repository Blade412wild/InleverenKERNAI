using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class NinjaBehaviour : MonoBehaviour
{
    public GameObject Player;
    public GameObject Guard;
    public Transform GuardAim;
    public Collider GuardCollider;
    public GameObject HideSpot;
    public Animator GuardAnimator;
    public GameObject OwnEyes;
    public GameObject[] HideSpots;
    [SerializeField] private bool playerIsVisible = false;
    private float timer = 1;
    private bool waitingIsDone = true;
    private NavMeshAgent guardAgent;
    [SerializeField] private bool guardIsVisible = false;
    //[SerializeField] private float Range;

    BehaviourTree mainTree;
    private NavMeshAgent agent;

    Node.Status treeStatus = Node.Status.Running;
    public enum ActionState { IDLE, WORKING };
    private ActionState state = ActionState.IDLE;



    // Start is called before the first frame update
    void Start()
    {
        /////////////////////////////////////////////////////////////////
        // main tree
        mainTree = new BehaviourTree("main Tree : ");
        agent = GetComponent<NavMeshAgent>();
        guardAgent = Guard.GetComponent<NavMeshAgent>();
        Selector beANinja = new Selector("Be A Ninja");


        // follow Player Tree
        Sequence followPlayer = new Sequence(" follow player");
        Sequence tryToMovePlayer = new Sequence(" try to Move Player");
        Leaf calculateTargetPos = new Leaf(" Calculate TargetPos", CalculateTargetPos);
        Leaf moveToPlayer = new Leaf(" go to player", GoToPlayer);
        Leaf checkifPlayerIsVisble = new Leaf(" check if player is visble", CheckIfPlayerIsVisble);



        // Protect Player tree
        Sequence protectPlayer = new Sequence(" protect Player");
        Leaf tryToFindAHideSpot = new Leaf(" try to find a hiding spot", TryToFindNearestHideSpot);
        Leaf moveToNearestHideSPot = new Leaf(" try go to nearest hide spot", GoToNearestPoint);
        Leaf isGuardInSight = new Leaf("check if Guard is in sight", CheckIfGuardIsInSight);
        Leaf stunGuard = new("stun Guard", CheckGuard);
        Leaf waitforafewSeconds = new Leaf(" wair for a few seconds", WaitForFewSeconds);



        ///////////////////////////////////////////////////////////////////
        /// main tree

        beANinja.AddChild(followPlayer);
        beANinja.AddChild(protectPlayer);
        mainTree.AddChild(beANinja);


        ////////////////////////////////////////////////////
        /// follow Player Tree
        tryToMovePlayer.AddChild(checkifPlayerIsVisble);
        tryToMovePlayer.AddChild(calculateTargetPos);
        tryToMovePlayer.AddChild(moveToPlayer);
        followPlayer.AddChild(tryToMovePlayer);


        


        ////////////////////////////////////////////////////
        /// Protect Player tree

        GuardSight.OnPlayerSeen += UpdatePlayerLastSeenPos;
        GuardSight.OnPlayerNotSeen += UpdatePlayeroutOfSight;
        protectPlayer.AddChild(tryToFindAHideSpot);
        protectPlayer.AddChild(moveToNearestHideSPot);
        protectPlayer.AddChild(isGuardInSight);
        protectPlayer.AddChild(stunGuard);
        protectPlayer.AddChild(waitforafewSeconds);


        mainTree.PrintTree();

    }


    // Update is called once per frame
    void Update()
    {
        treeStatus = mainTree.Process();
        if (waitingIsDone == false)
        {
            Timer();
        }

    }
    private void FixedUpdate()
    {
        Transform target = GuardAim.transform;
        Vector3 dirToTarget = (target.position - OwnEyes.transform.position).normalized;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(OwnEyes.transform.position, dirToTarget, out hit, 50) && GuardCollider == hit.collider)
        {
            Debug.DrawRay(OwnEyes.transform.position, dirToTarget, Color.yellow);
            guardIsVisible = true;
        }
        else
        {
            Debug.DrawRay(OwnEyes.transform.position, dirToTarget, Color.red);

            guardIsVisible = false;
        }
    }

    private void Timer()
    {
        float seconds = Time.deltaTime;
        timer = timer + seconds;
        Debug.Log("timer = " + timer);

        if(timer >= 2.5)
        {
            UndoStun();
        }

        if(timer >= 5)
        {
            waitingIsDone = true;
        }


        
    }

    private Node.Status CheckGuard()
    {
        if(waitingIsDone == false)
        {
            return Node.Status.Failed;
        }
        else
        {
            StunGuard();
            return Node.Status.Succes;
        }
    }

    private void StunGuard()
    {
        guardAgent.speed = 0;
        GuardAnimator.SetTrigger("GuardIsHit");
    }

    private void UndoStun()
    {
        guardAgent.speed = 4.0f;
    }

    private Node.Status WaitForFewSeconds()
    {
        timer = 0;
        waitingIsDone = false;
        return Node.Status.Succes;
    }

    private void UpdatePlayerLastSeenPos(Vector3 _playerLastSeenPos)
    {
        playerIsVisible = true;
    }
    private void UpdatePlayeroutOfSight(Vector3 _playerSight)
    {
        playerIsVisible = false;
    }
    private Node.Status CheckIfGuardIsInSight()
    {
        if(guardIsVisible == true)
        {
            return Node.Status.Succes;
        }
        else
        {
            return Node.Status.Failed;
        }
    }
    public Node.Status GoToNearestPoint()
    {
        GameObject nearestPoint = null;
        float lowestDistance = 10000000.0f;
        float distance;

        for (int i = 0; i < HideSpots.Length; i++)
        {
            distance = Vector3.Distance(Player.transform.position, HideSpots[i].transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                nearestPoint = HideSpots[i];
            }

        }

        return GoToLocation(nearestPoint.transform.position);
    }
    private Node.Status TryToGetToNearestPoint()
    {
        return GoToLocation(HideSpot.transform.position);
    }
    private Node.Status CheckIfPlayerIsVisble()
    {
        if (playerIsVisible == true)
        {
            return Node.Status.Failed;
        }
        else
        {
            return Node.Status.Succes;
        }
    }

    private Node.Status GoToPlayer()
    {
        if (CheckIfPlayerIsVisble() == Node.Status.Failed)
        {
            return Node.Status.Failed;
        }
        else
        {
            return GoToLocation(Player.transform.position);
        }
    }

    Node.Status GoToLocation(Vector3 _destination)
    {
        Debug.Log(" state = " + state);
        float distanceToTarget = Vector3.Distance(_destination, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(_destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, _destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.Running;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.Succes;
        }

        return Node.Status.Running;
    }

    private Node.Status CalculateTargetPos()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.0f)
        {
            return Node.Status.Running;
        }
        else
        {
            return Node.Status.Succes;
        }
    }

    private Node.Status TryToFindNearestHideSpot()
    {
        return Node.Status.Succes;
    }
}
