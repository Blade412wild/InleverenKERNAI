using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObject : MonoBehaviour
{
    public GameObject BulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GateManager.OnOpenMainGate += Shoot;
    }

    private void Shoot()
    {
        Debug.Log(" shoot");
        GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * 100f);
    }

    private void OnDisable()
    {
        GateManager.OnOpenMainGate -= Shoot;
    }
}
