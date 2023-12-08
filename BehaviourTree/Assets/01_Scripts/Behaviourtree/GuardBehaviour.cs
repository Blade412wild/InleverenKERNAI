using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : MonoBehaviour
{
    BehaviourTree mainTree;
    [SerializeField] private Animator animator;

    [Header("patrol")]
    public NormalPatrolPoints[] normalPatrolPointsArray;
    [SerializeField] private float walkSpeed;

    private enum path { normal, special }
    [SerializeField] private path currentPath;
    private NavMeshAgent agent;
    private int currentTarget;

    [Header("Chase")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject WeaponPickUp;
    [SerializeField] private GameObject WeaponOnCharacter;
    [SerializeField] private GameObject[] weaponArray;
    private GameObject weaponTarget;
    public delegate void Guard(bool outcome);
    public Guard OnStartWeaponPickUp;
    public Guard OnEndWeaponPickUp;
    private bool GuardPickUpsWeapon = false;
    [SerializeField] private bool playerIsVisible = false;
    [SerializeField] private bool guardHasWeapon = false;
    private Vector3 playerLastSeenPos;
    [SerializeField] private float runSpeed;





    [Header("Attack")]
    [Range(0, 7)][SerializeField] private float hitRange = 1.0f;
    [SerializeField] private int hitDamage = 50;
    [SerializeField] private int attackTime = 5;
    private bool haveWaited = false;

    public delegate void GuardAttack(int damage);
    public static event GuardAttack OnAttack;


    public enum ActionState { IDLE, WORKING };
    private ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.Running;
    Node.Status PatrolStatus = Node.Status.Running;


    // Start is called before the first frame update
    void Start()
    {
        WeaponOnCharacter.SetActive(false);
        agent = GetComponent<NavMeshAgent>();

        /////////////////////////////////////////////////////////////////
        // main tree
        mainTree = new BehaviourTree("main Tree : ");
        Selector guardTheMaze = new Selector("Guard The Maze");
        Leaf checkIfPlayerIsVisble = new Leaf("Check if Player is visble", CheckIfPlayerIsVisble);

        // Patrouileer tree
        Sequence patrolTree = new Sequence("patrol");
        Sequence moveThroughMaze = new Sequence("Walk Through maze");
        Selector checkNearPoint = new Selector("try to Check if Near point");
        Selector checkKindOfPoint = new Selector("Check Which point");
        Leaf moveToNextPoint = new Leaf("move to next point", GoToNextPoint);
        Leaf checkIfNearAnPoint = new Leaf("check if Near a point", ChecKIfNearPoint);
        Leaf moveToNearestPoint = new Leaf("move to nearest point", GoToNearestPoint);
        Leaf think = new Leaf("think", Think);
        Leaf playWalkAnimation = new Leaf("play walk Animation", PlayWalkAnimation);
        Leaf setwalkSpeed = new Leaf("set walk speed", SetWalkingSpeed);

        // Chase Tree
        Sequence chaseTree = new Sequence("Chase");
        Selector getWeapon = new Selector("get Weapon");
        Sequence tryToGetWeapon = new Sequence("Trying to get Weapon");
        Leaf startGettingWeaponEvent = new Leaf("Start Get Weapon Event", StartWeaponPickUpEvent);
        Leaf endGettingWeaponEvent = new Leaf("End Get Weapon Event", EndWeaponPickUpEvent);
        Leaf useHands = new Leaf("Use no Weapon", UseHands);
        Leaf searchforWeapon = new Leaf("Search for Weapon", SearchNearestWeapon);
        Leaf moveToWeapon = new Leaf("move to Weapon", GoToWeapon);
        Leaf checkForWeaponInHand = new Leaf("Check for a Weapon", CheckForWeaponInHand);
        Leaf moveToPlayer = new Leaf("Move to Player", GoToPlayer);
        Leaf playRunAnimation = new Leaf("play run animation", PlayRunAnimation);
        Leaf setRunSpeed = new Leaf("Set Guard Speed", SetRunSpeed);

        // attack tree
        Sequence tryToAttackPlayer = new Sequence("try to attack the palyer");
        Leaf checkIfPlayerIsInRange = new Leaf("Check if Player is in Range", CheckIfPlayerIsInRange);
        Leaf playAttackAnimation = new Leaf("play Attack animantion", PlayAttackAnimation);
        Leaf doDamage = new Leaf("Do Damge : " + hitDamage, DoDamage);
        Leaf wait2second = new Leaf("Wait for 2 second", Waitfor2Second);


        ///////////////////////////////////////////////////////////////////
        /// main tree

        guardTheMaze.AddChild(patrolTree);
        guardTheMaze.AddChild(chaseTree);
        mainTree.AddChild(guardTheMaze);



        ////////////////////////////////////////////////////
        /// patrol tree

        checkNearPoint.AddChild(checkIfNearAnPoint);
        checkNearPoint.AddChild(moveToNearestPoint);
        moveThroughMaze.AddChild(checkNearPoint);
        moveThroughMaze.AddChild(checkIfPlayerIsVisble);
        moveThroughMaze.AddChild(think);
        moveThroughMaze.AddChild(moveToNextPoint);
        //patrolTree.AddChild(checkIfPlayerIsVisble);
        patrolTree.AddChild(playWalkAnimation);
        patrolTree.AddChild(setwalkSpeed);
        patrolTree.AddChild(moveThroughMaze);



        ////////////////////////////////////////////////////
        /// Chase tree
        GuardSight.OnPlayerSeen += UpdatePlayerLastSeenPos;
        GuardSight.OnPlayerNotSeen += UpdatePlayeroutOfSight;
        OnStartWeaponPickUp += ChangeWeaponBool;
        OnEndWeaponPickUp += ChangeWeaponBool;

        tryToGetWeapon.AddChild(startGettingWeaponEvent);
        tryToGetWeapon.AddChild(searchforWeapon);
        tryToGetWeapon.AddChild(moveToWeapon);
        tryToAttackPlayer.AddChild(checkIfPlayerIsInRange);
        tryToAttackPlayer.AddChild(playAttackAnimation);
        tryToAttackPlayer.AddChild(doDamage);
        //tryToAttackPlayer.AddChild(wait2second);
        tryToGetWeapon.AddChild(endGettingWeaponEvent);
        getWeapon.AddChild(checkForWeaponInHand);
        getWeapon.AddChild(tryToGetWeapon);
        getWeapon.AddChild(useHands);
        chaseTree.AddChild(playRunAnimation);
        chaseTree.AddChild(setRunSpeed);
        chaseTree.AddChild(getWeapon);
        chaseTree.AddChild(moveToPlayer);
        chaseTree.AddChild(tryToAttackPlayer);
        chaseTree.AddChild(endGettingWeaponEvent);



        mainTree.PrintTree();
    }

    private Node.Status Waitfor2Second()
    {

        Debug.Log(" you're dead");
        return Node.Status.Succes;
    }

    private Node.Status CheckIfPlayerIsInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= hitRange)
        {
            return Node.Status.Succes;
        }
        else
        {
            return Node.Status.Failed;
        }
    }
    private Node.Status SetRunSpeed()
    {
        return SetSpeed(runSpeed);
    }
    private Node.Status SetWalkingSpeed()
    {
        return SetSpeed(walkSpeed);
    }

    private Node.Status SetSpeed(float _speed)
    {
        agent.speed = _speed;
        return Node.Status.Succes;
    }
    private Node.Status DoDamage()
    {
        OnAttack?.Invoke(hitDamage);
        return Node.Status.Succes;
    }
    private Node.Status PlayWalkAnimation()
    {
        //animator.SetBool();
        animator.SetBool("GuardIsChasing", false);
        return Node.Status.Succes;
    }
    private Node.Status PlayRunAnimation()
    {
        Debug.Log(" Play Run animation");
        animator.SetBool("GuardIsChasing", true);
        return Node.Status.Succes;
    }
    private Node.Status PlayAttackAnimation()
    {
        animator.SetTrigger("GuardAttacks");

        return Node.Status.Succes;
    }

    private Node.Status UseHands()
    {
        Debug.Log("use hands");

        return Node.Status.Succes;
    }
    public Node.Status StartWeaponPickUpEvent()
    {
        OnStartWeaponPickUp?.Invoke(false);
        state = ActionState.IDLE;
        return Node.Status.Succes;
    }
    public Node.Status EndWeaponPickUpEvent()
    {
        OnStartWeaponPickUp?.Invoke(true);
        return Node.Status.Succes;
    }
    private void ChangeWeaponBool(bool _outcome)
    {
        guardHasWeapon = _outcome;
        Debug.Log("Guard pick Up Weapon = " + GuardPickUpsWeapon);
    }
    private void ChangePlayerVisbleBool(bool _outcome)
    {
        playerIsVisible = _outcome;
        Debug.Log("PlayerIsVisble = " + playerIsVisible);
    }
    private Node.Status CheckForWeaponInHand()
    {
        if (guardHasWeapon == false)
        {
            return Node.Status.Failed;
        }
        else
        {
            return Node.Status.Succes;
        }
    }
    private Node.Status SearchNearestWeapon()
    {
        GameObject nearestPoint = null;
        float lowestDistance = 10000000.0f;
        float distance;

        for (int i = 0; i < weaponArray.Length; i++)
        {
            distance = Vector3.Distance(transform.position, weaponArray[i].transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                weaponTarget = weaponArray[i];
            }

        }

        Debug.Log("nearest weapon = " + weaponTarget.transform.name + " pos = " + weaponTarget.transform.position);
        return Node.Status.Succes;
    }
   
    private void UpdatePlayerLastSeenPos(Vector3 _playerLastSeenPos)
    {
        playerIsVisible = true;
        playerLastSeenPos = _playerLastSeenPos;
        //animator.SetBool("GuardIsChasing", true); 
        Debug.Log("Player last seen pos = " + playerLastSeenPos);
    }
    private void UpdatePlayeroutOfSight(Vector3 _playerSight)
    {
        playerIsVisible = false;
        //animator.SetBool("GuardIsChasing", false);
    }
    private Node.Status GoToPlayer()
    {
        return GoToLocation(playerLastSeenPos);
    }
    private Node.Status GoToWeapon()
    {
        Node.Status status = GoToLocation(weaponTarget.transform.position);

        if (status == Node.Status.Succes)
        {
            weaponTarget.SetActive(false);
            WeaponOnCharacter.SetActive(true);
        }
        return status;
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
    public Node.Status Think()
    {
        if (normalPatrolPointsArray[currentTarget].Route == NormalPatrolPoints.PatrolRoute.Normal)
        {
            Debug.Log("current target is " + currentTarget + " count : " + normalPatrolPointsArray.Count());
            if (currentTarget == normalPatrolPointsArray.Count() - 1)
            {
                currentTarget = 0;
            }
            else
            {
                currentTarget++;
            }
        }
        else if (normalPatrolPointsArray[currentTarget].Route == NormalPatrolPoints.PatrolRoute.Special)
        {

        }

        return Node.Status.Succes;
    }
    public Node.Status GoToNextPoint()
    {
        Debug.Log(currentTarget);
        if (CheckIfPlayerIsVisble() == Node.Status.Failed)
        {
            Debug.Log(" GoToNextPoint is failed");
            return Node.Status.Failed;

        }
        else
        {
            Debug.Log(" GoToNextPoint isn't failed");
            return GoToLocation(normalPatrolPointsArray[currentTarget].transform.position);
        }

    }
    public Node.Status GoToNearestPoint()
    {
        NormalPatrolPoints nearestPoint = null;
        float lowestDistance = 10000000.0f;
        float distance;

        for (int i = 0; i < normalPatrolPointsArray.Length; i++)
        {
            distance = Vector3.Distance(transform.position, normalPatrolPointsArray[i].Trans.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                nearestPoint = normalPatrolPointsArray[i];
                currentTarget = i;
            }

        }

        return GoToLocation(nearestPoint.Trans.position);
    }

    public Node.Status ChecKIfNearPoint()
    {
        bool guardIsNearAPoint = false;
        float distance;
        float succesDistance = 2f;

        for (int i = 0; i < normalPatrolPointsArray.Length; i++)
        {
            distance = Vector3.Distance(transform.position, normalPatrolPointsArray[i].Trans.position);
            if (distance <= succesDistance)
            {
                currentTarget = i;
                guardIsNearAPoint = true;
            }

            Debug.Log("Patrolpoint : " + normalPatrolPointsArray[i].Trans.name + "is " + guardIsNearAPoint);

        }

        if (guardIsNearAPoint == true)
        {
            return Node.Status.Succes;
        }
        else
        {
            return Node.Status.Failed;
        }
    }

    public bool CheckDestinationReached(Vector3 _begin, Vector3 _destination, float _succesDistance)
    {
        if (Vector3.Distance(_begin, _destination) < _succesDistance)
        {
            return true;
        }

        return false;
    }

    Node.Status GoToLocation(Vector3 _destination)
    {
        float distanceToTarget = Vector3.Distance(_destination, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(_destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, _destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.Failed;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.Succes;
        }

        return Node.Status.Running;
    }


    // Update is called once per frame
    void Update()
    {
        treeStatus = mainTree.Process();

    }

    private void OnDisable()
    {
        OnEndWeaponPickUp -= ChangeWeaponBool;
        OnStartWeaponPickUp -= ChangeWeaponBool;
    }
}
