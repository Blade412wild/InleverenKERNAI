using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Socket;

public class Plug : MonoBehaviour
{
    public enum PlugType {purple, Orange, Green, Blue};
    public PlugType plugType;

    public Transform PlugTrans;
    public Rigidbody rigidbody;
    public bool IsPickable;
    private bool PlugHasBeenPickedUp;

    // Start is called before the first frame update
    void Start()
    {
        IsPickable = true;
        rigidbody = GetComponent<Rigidbody>();
        PlugTrans = GetComponent<Transform>();

        Socket.OnplughasBeenPlaced += IsPickableToFalse;
    }

    private void IsPickableToFalse(Socket.SocketType type)
    {
        switch (type)
        {
            case SocketType.purple:
                if (plugType == PlugType.purple)
                {
                    IsPickable = false;
                }
                break;

            case SocketType.Orange:
                if (plugType == PlugType.Orange)
                {
                    IsPickable = false;
                }
                break;

            case SocketType.Green:
                if (plugType == PlugType.Green)
                {
                    IsPickable = false;
                }

                break;

            case SocketType.Blue:
                if (plugType == PlugType.Blue)
                {
                    IsPickable = false;
                }

                break;
        }
    }

    private void OnDisable()
    {
        Socket.OnplughasBeenPlaced -= IsPickableToFalse;
    }


}
