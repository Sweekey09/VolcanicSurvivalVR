using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public static StatsTracker Instance { get; private set; }

    [Header("Tracked Stats")]
    public int playerScore = 0;
    public int enemiesKilled = 0;
    public int shotsFired = 0;
    public int shotsHit = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public float AccuracyPercent
    {
        get
        {
            if (shotsFired <= 0) return 0f;
            return (shotsHit / (float)shotsFired) * 100f;
        }
    }

    public void ResetStats()
    {
        playerScore = 0;
        enemiesKilled = 0;
        shotsFired = 0;
        shotsHit = 0;
    }
}
