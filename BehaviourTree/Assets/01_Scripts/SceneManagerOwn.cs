using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerOwn : MonoBehaviour
{
    public float ReloadSceneTime;
    private Scene currentScene;

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Enviroment1");
    }

}
