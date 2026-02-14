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

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // ✅ Call Excel export instead of CSV
            WriteToExcel excel = FindObjectOfType<WriteToExcel>();
            if (excel != null)
            {
                excel.EndTestAndWriteRecord();
                Debug.Log("Excel record written.");
            }
            else
            {
                Debug.LogError("WriteToExcel not found in scene!");
            }

            SceneManager.LoadScene(winSceneName);
        }
    }
}
