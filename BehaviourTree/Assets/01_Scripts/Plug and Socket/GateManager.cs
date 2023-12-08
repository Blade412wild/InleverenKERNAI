using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    public GameObject FrontGate;
    public GameObject CenterGate;
    public GameObject MainGate;

    private bool SocketPurple;
    private bool SocketOrange;

    public delegate void SocketActivated();
    public static event SocketActivated OnOpenMainGate;


    // Start is called before the first frame update
    void Start()
    {
        Socket.OnplughasBeenPlaced += GateHandler;
    }
    private void GateHandler(Socket.SocketType _socketType)
    {
        switch (_socketType)
        {
            case Socket.SocketType.Blue: OpenFrontGate(); break;
            case Socket.SocketType.Green: OpenMainGate(); break;
            case Socket.SocketType.purple: SocketPurple = true; OpenCenterGate(); break;
            case Socket.SocketType.Orange: SocketOrange = true; OpenCenterGate(); break;
        }


    }

    private void OpenFrontGate()
    {
        Destroy(FrontGate);
    }

    private void OpenMainGate()
    {
        OnOpenMainGate?.Invoke();
    }

    private void OpenCenterGate()
    {
        if (SocketOrange == true && SocketPurple == true)
        {
            Destroy(CenterGate);
        }
    }
    private void OnDisable()
    {
        Socket.OnplughasBeenPlaced -= GateHandler;
    }
}
