using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIUpdater : MonoBehaviour
{
    public GameObject EndScreen;
    public GameObject PlayerUICanvas;
    public TMP_Text PlayerHealth;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EndScreen.SetActive(false);

        Player.OnPlayerUpdate += UpdatePlayerUI;
        Player.OnPlayerDeath += ShowEndScreen;

    }

    private void UpdatePlayerUI(int _health)
    {
        PlayerHealth.text = "Player Health = " + _health.ToString();
    }

    private void ShowEndScreen(int ouctome)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerUICanvas.SetActive(false);
        EndScreen.SetActive(true);

    }

    private void OnDisable()
    {
        Player.OnPlayerUpdate -= UpdatePlayerUI;
        Player.OnPlayerDeath -= ShowEndScreen;
    }
}
