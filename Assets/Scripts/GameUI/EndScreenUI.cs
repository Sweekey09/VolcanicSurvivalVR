using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string menuSceneName = "MenuScene";
    public string gameSceneName = "GameScene";

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QuitGame() called (works in build, not in editor).");
    }
}
