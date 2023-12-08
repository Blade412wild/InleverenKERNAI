using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Socket : MonoBehaviour
{
    public Transform PlugTrans;
    public enum SocketType { purple, Orange, Green, Blue };
    public SocketType socketType;

    public delegate void PlughasBeenPlaced(SocketType socketType);
    public static event PlughasBeenPlaced OnplughasBeenPlaced;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {

        Plug plugScript = collision.gameObject.GetComponent<Plug>();
        if (plugScript != null)
        {
            Plug.PlugType plugType;
            plugType = plugScript.plugType;

            switch (plugType)
            {
                case Plug.PlugType.purple:
                    if (socketType == SocketType.purple)
                    {
                        SetPositionAndRigigdBody(collision);
                        Debug.Log("purple is on the right socket");
                        OnplughasBeenPlaced?.Invoke(SocketType.purple);
                    }
                    else
                    {
                        Debug.Log("wrong Socket");
                    }
                    break;

                case Plug.PlugType.Orange:
                    if (socketType == SocketType.Orange)
                    {
                        SetPositionAndRigigdBody(collision);
                        Debug.Log("Orange is on the right socket");
                        OnplughasBeenPlaced?.Invoke(SocketType.Orange);
                    }
                    else
                    {
                        Debug.Log("wrong Socket");
                    }
                    break;

                case Plug.PlugType.Green:
                    if (socketType == SocketType.Green)
                    {
                        SetPositionAndRigigdBody(collision);
                        Debug.Log("Green is on the right socket");
                        OnplughasBeenPlaced?.Invoke(SocketType.Green);
                    }
                    else
                    {
                        Debug.Log("wrong Socket");
                    }
                    break;

                case Plug.PlugType.Blue:
                    if (socketType == SocketType.Blue)
                    {
                        SetPositionAndRigigdBody(collision);
                        Debug.Log("Blue is on the right socket");
                        OnplughasBeenPlaced?.Invoke(SocketType.Blue);
                    }
                    else
                    {
                        Debug.Log("wrong Socket");
                    }
                    break;
            }

        }
    }
    private void SetPositionAndRigigdBody(Collision _collision)
    {

        _collision.rigidbody.velocity = Vector3.zero;
        _collision.rigidbody.angularVelocity = Vector3.zero;
        _collision.transform.position = PlugTrans.position;
        _collision.transform.rotation = PlugTrans.rotation;

        Destroy(_collision.rigidbody);
    }
}

