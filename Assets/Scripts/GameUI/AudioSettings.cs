using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance { get; private set; }

    public float musicVolume = 0.7f;

    const string MUSIC_KEY = "musicVolume";

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();

        // Load saved value
        musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, musicVolume);

        ApplyVolume();
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);

        PlayerPrefs.SetFloat(MUSIC_KEY, musicVolume);
        PlayerPrefs.Save();

        ApplyVolume();
    }

    void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
}