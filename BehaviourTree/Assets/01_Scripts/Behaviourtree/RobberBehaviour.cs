using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;

    public GameObject Diamond;
    public GameObject Van;
    public GameObject FrontDoor;
    public GameObject BackDoor;
    private NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING };
    private ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.Running;

    [Range(0, 1000)]
    public int Money = 800;




    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree("Robber behaviour tree : ");
        Sequence steal = new Sequence("steal something");
        Leaf goToDiamant = new Leaf("Go To Diamont", GoToDiamont);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Leaf checkMoney = new Leaf("Check Money", CheckMoney);
        Selector openDoor = new Selector("Open Door");

        // tree
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        steal.AddChild(checkMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToDiamant);
        steal.AddChild(goToVan);
        tree.AddChild(steal);


        //Node eat = new Node("eat something");
        //Node pizza = new Node("Go To Pizza Shop");
        //Node buy = new Node("Buy Pizza");

        //eat.AddChild(pizza);
        //eat.AddChild(buy);
        //tree.AddChild(eat);



        tree.PrintTree();

    }
    public Node.Status CheckMoney()
    {
        if(Money >= 500)
        {
            return Node.Status.Failed;
        }

        return Node.Status.Succes;
    }
    public Node.Status GoToDiamont()
    {
        return GoToLocation(Diamond.transform.position);
    }
    public Node.Status GoToVan()
    {
        return GoToLocation(Van.transform.position);
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(BackDoor);
    }
    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(FrontDoor);
    }

    public Node.Status GoToDoor(GameObject _door)
    {
        Node.Status status = GoToLocation(_door.transform.position);

        if (status == Node.Status.Succes)
        {
            if (!_door.GetComponent<Lock>().Islocked)
            {
                _door.SetActive(false);
                return Node.Status.Succes;
            }
            return Node.Status.Failed;
        }
        else
        {
            return status;
        }

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
        if (treeStatus != Node.Status.Succes)
        {
            treeStatus = tree.Process();
        }
    }
}
