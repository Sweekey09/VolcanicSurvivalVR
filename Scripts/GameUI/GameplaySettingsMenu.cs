using UnityEngine;

public class GameplaySettingsMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;

    [Header("Optional: disable enemy spawns while paused")]
    public MonoBehaviour waveManager; // drag your WaveManager here (optional)

    bool isOpen;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        ResumeGame(); // ensure timeScale normal
    }

    public void OpenSettings()
    {
        isOpen = true;
        if (settingsPanel != null) settingsPanel.SetActive(true);

        Time.timeScale = 0f;              // pause game
        if (waveManager != null) waveManager.enabled = false;
    }

    public void CloseSettings()
    {
        isOpen = false;
        if (settingsPanel != null) settingsPanel.SetActive(false);

        ResumeGame();
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        if (waveManager != null) waveManager.enabled = true;
    }
}
