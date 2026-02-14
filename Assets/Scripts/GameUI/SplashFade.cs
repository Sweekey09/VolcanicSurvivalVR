using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashFade : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "CA1MenuScene";

    [Header("Timings")]
    public float fadeInTime = 1.0f;
    public float holdTime = 1.0f;
    public float fadeOutTime = 1.0f;

    [Header("Optional")]
    public bool allowSkipWithAnyKey = false;

    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    IEnumerator Start()
    {
        // Fade in
        yield return FadeTo(1f, fadeInTime);

        // Hold
        float t = 0f;
        while (t < holdTime)
        {
            if (allowSkipWithAnyKey && Input.anyKeyDown) break;
            t += Time.deltaTime;
            yield return null;
        }

        // Fade out
        yield return FadeTo(0f, fadeOutTime);

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = cg.alpha;
        if (duration <= 0f)
        {
            cg.alpha = target;
            yield break;
        }

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        cg.alpha = target;
    }
}
