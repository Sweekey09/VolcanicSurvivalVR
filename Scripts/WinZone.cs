using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour
{
    [Header("Scene to load on win")]
    public string winSceneName = "WinScene";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Works for VR rigs: XR Origin root OR CharacterController child
        if (other.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null)
        {
            triggered = true;
            SceneManager.LoadScene(winSceneName);
        }
    }
}
