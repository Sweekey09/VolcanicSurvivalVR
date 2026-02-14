using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;

    void OnEnable()
    {
        if (AudioSettings.Instance == null || musicSlider == null)
            return;

        // Set slider value to saved music volume
        musicSlider.SetValueWithoutNotify(AudioSettings.Instance.musicVolume);

        // Avoid duplicate listeners
        musicSlider.onValueChanged.RemoveAllListeners();

        // Add listener for music volume only
        musicSlider.onValueChanged.AddListener(AudioSettings.Instance.SetMusicVolume);
    }
}
