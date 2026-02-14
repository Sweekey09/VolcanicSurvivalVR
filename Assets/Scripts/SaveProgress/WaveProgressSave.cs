using UnityEngine;

public class WaveProgressSave : MonoBehaviour
{
    private const string WaveKey = "SavedWave";

    // Save the wave the player is currently on (1,2,3...)
    public void SaveWave(int currentWave)
    {
        PlayerPrefs.SetInt(WaveKey, currentWave);
        PlayerPrefs.Save();
        Debug.Log("Saved Wave = " + currentWave);
    }

    // Returns saved wave; default is 1 (new game)
    public int LoadWave()
    {
        int w = PlayerPrefs.GetInt(WaveKey, 1);
        Debug.Log("Loaded Wave = " + w);
        return w;
    }

    // Optional: restart from Wave 1
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(WaveKey);
        PlayerPrefs.Save();
        Debug.Log("Cleared saved wave");
    }
}
