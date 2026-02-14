using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoader : MonoBehaviour
{
    public string winSceneName = "WinScene";

    // Call this when player wins (all waves cleared, objective completed, etc.)
    public void LoadWinScene()
    {
        SceneManager.LoadScene(winSceneName);
    }
}
