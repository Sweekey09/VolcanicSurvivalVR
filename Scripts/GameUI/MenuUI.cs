using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Scenes")]
    public string gameplaySceneName = "Gameplay";   // change to your gameplay scene name
    public string loadingSceneName = "";            // optional, leave empty if not using

    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject settingsPanel;

    public void OnClickStart()
    {
        if (!string.IsNullOrEmpty(loadingSceneName))
            SceneManager.LoadScene(loadingSceneName);
        else
            SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnClickSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    public void OnClickBackFromSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Quit Game (will only quit in build)");
    }
}
