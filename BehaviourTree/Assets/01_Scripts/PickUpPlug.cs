using UnityEngine;

public class PickUpPlug : MonoBehaviour
{
    public KeyCode InputKey;
    public Transform PickUpItemTrans;
    [SerializeField] private bool PlugHasBeenPickedUp;
    private Plug plug;

    //private delegate void PickUp();
    //private event PickUp OnpickUp;
    //private event PickUp OnRelease;

    // Start is called before the first frame update
    void Start()
    {
        PlugHasBeenPickedUp = false;
        Socket.OnplughasBeenPlaced += ResetPlug;
        //OnpickUp += PickUp;
        //OnRelease += Release;
    }

    // Update is called once per frame
    void Update()
    {


        if (plug != null && PlugHasBeenPickedUp == true)
        {
            PickUp();
        }

        if (PlugHasBeenPickedUp == true)
        {
            if (Input.GetKeyDown(InputKey))
            {
                ResetRigidBody();
                PlugHasBeenPickedUp = false;
            }
        }

        if (PlugHasBeenPickedUp == false)
        {
            if (Input.GetKeyDown(InputKey))
            {
                plug = CheckForPlug();
                if (plug != null)
                {
                    //plug = null;
                    PlugHasBeenPickedUp = true;
                }
            }
        }



    }

    private Plug CheckForPlug()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            Plug plug = hit.transform.gameObject.GetComponent<Plug>();
            if (plug != null && plug.IsPickable == true)
            {
                Debug.Log("hit a plug");
                return plug;
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    private void PickUp()
    {
        if (plug != null)
        {
            plug.transform.position = PickUpItemTrans.position;
        }
    }

    private void ResetRigidBody()
    {
        plug.rigidbody.velocity = Vector3.zero;
        plug.rigidbody.angularVelocity = Vector3.zero;
    }

    private void ResetPlug(Socket.SocketType type)
    {
        plug = null;
        PlugHasBeenPickedUp = false;
    }

    private void OnDisable()
    {
        Socket.OnplughasBeenPlaced -= ResetPlug;
    }


}
