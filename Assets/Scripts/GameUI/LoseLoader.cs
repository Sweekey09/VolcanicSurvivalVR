using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseLoader : MonoBehaviour
{
    public string loseSceneName = "LoseScene";

    public void LoadLoseScene()
    {
        SceneManager.LoadScene(loseSceneName);
    }
}
