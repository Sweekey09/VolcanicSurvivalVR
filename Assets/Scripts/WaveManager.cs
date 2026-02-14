using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Waves (Level 1-5)")]
    public int[] enemiesPerWave = new int[] { 2, 3, 4, 5, 6 };
    public float timeBetweenSpawns = 0.5f;
    public float timeBetweenWaves = 2f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public float waveTextDuration = 2f;

    [Header("Win Scene")]
    public WinLoader winLoader; // drag your GameManager (with WinLoader) here
    public bool clearSaveOnWin = true;

    private int currentWaveIndex = -1;
    private int aliveCount = 0;

    private Coroutine waveTextRoutine;

    // save system reference
    private WaveProgressSave waveSave;

    void Start()
    {
        // get saver + load saved wave
        waveSave = GetComponent<WaveProgressSave>();

        int savedWave = (waveSave != null) ? waveSave.LoadWave() : 1; // 1..N
        currentWaveIndex = savedWave - 2; // so StartNextWave() will ++ to correct wave

        StartNextWave();
    }

    void StartNextWave()
    {
        currentWaveIndex++;

        // ✅ WIN CONDITION: finished all waves
        if (currentWaveIndex >= enemiesPerWave.Length)
        {
            Debug.Log("All waves cleared! YOU WIN!");

            // optional: clear save when game finished
            if (clearSaveOnWin && waveSave != null)
                waveSave.ClearSave();

            // load win scene
            if (winLoader != null)
            {
                winLoader.LoadWinScene();
            }
            else
            {
                Debug.LogError("WinLoader not assigned in WaveManager. Assign it in Inspector!");
                // fallback (optional): direct load if you want
                // SceneManager.LoadScene("WinScene");
            }

            return;
        }

        int waveNumber = currentWaveIndex + 1;

        // save progress at the start of each wave
        if (waveSave != null) waveSave.SaveWave(waveNumber);

        // show wave text
        ShowWaveText(waveNumber);

        StartCoroutine(SpawnWaveCoroutine(enemiesPerWave[currentWaveIndex]));
    }

    void ShowWaveText(int waveNumber)
    {
        if (waveText == null) return;

        if (waveTextRoutine != null)
            StopCoroutine(waveTextRoutine);

        waveTextRoutine = StartCoroutine(ShowWaveTextCoroutine(waveNumber));
    }

    IEnumerator SpawnWaveCoroutine(int count)
    {
        Debug.Log($"Wave {currentWaveIndex + 1} start! Spawning {count} enemies...");
        yield return new WaitForSeconds(timeBetweenWaves);

        aliveCount = 0;

        for (int i = 0; i < count; i++)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnOneEnemy()
    {
        if (enemyData == null || enemyData.prefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemyData.prefab, sp.position, sp.rotation);

        aliveCount++;

        var eh = enemyGO.GetComponentInChildren<EnemyHealth>();
        if (eh != null)
        {
            eh.SetStats(enemyData, currentWaveIndex + 1);
            eh.OnDied += HandleEnemyDied;
        }
    }

    void HandleEnemyDied(EnemyHealth enemy)
    {
        aliveCount--;

        if (aliveCount <= 0)
        {
            Debug.Log($"Wave {currentWaveIndex + 1} cleared!");
            StartNextWave();
        }
    }

    IEnumerator ShowWaveTextCoroutine(int waveNumber)
    {
        waveText.gameObject.SetActive(true);
        waveText.text = $"Wave {waveNumber} Start";
        yield return new WaitForSeconds(waveTextDuration);
        waveText.gameObject.SetActive(false);
    }

    // call this from your Restart button (restart progress to wave 1 without reloading scene)
    public void RestartProgress()
    {
        if (waveSave != null) waveSave.ClearSave();

        StopAllCoroutines();
        aliveCount = 0;
        currentWaveIndex = -1;

        StartNextWave();
    }

    // call this if you want to save progress then return to menu
    public void SaveAndQuitToMenu()
    {
        int waveNumber = currentWaveIndex + 1;

        if (waveSave != null)
            waveSave.SaveWave(waveNumber);

        SceneManager.LoadScene("MenuScene"); // must match exact scene name
    }
}