using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // use this if you are using TextMeshPro

public class SimpleLoading : MonoBehaviour
{
    public Image fillBar;
    public TMP_Text percentText;   // percentage display (TextMeshPro)

    public float fakeLoadDuration = 3f;   // how many seconds the bar takes
    public string sceneToLoad = "Gameplay";

    private void Start()
    {
        StartCoroutine(FakeLoadingBar());
    }

    IEnumerator FakeLoadingBar()
    {
        float timer = 0f;

        while (timer < fakeLoadDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fakeLoadDuration);

            // Update fill bar
            if (fillBar != null)
                fillBar.fillAmount = progress;

            // Update percentage text
            if (percentText != null)
            {
                int percent = Mathf.RoundToInt(progress * 100f);
                percentText.text = percent + "%";
            }

            yield return null;
        }

        // Ensure it ends at 100%
        if (fillBar != null) fillBar.fillAmount = 1f;
        if (percentText != null) percentText.text = "100%";

        yield return new WaitForSeconds(0.2f);

        // Load next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}

