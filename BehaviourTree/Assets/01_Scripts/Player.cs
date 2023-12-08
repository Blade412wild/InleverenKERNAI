using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Health = 100;
    public delegate void PlayeruIUpdate(int Health);
    public static event PlayeruIUpdate OnPlayerUpdate;
    public static event PlayeruIUpdate OnPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        GuardBehaviour.OnAttack += DoDamge;
    }

    private void DoDamge(int _damage)
    {
        Health = -_damage;

        OnPlayerUpdate?.Invoke(Health);
        if (Health <= 0)
        {
            OnPlayerDeath?.Invoke(Health);
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        GuardBehaviour.OnAttack -= DoDamge;
    }
}
