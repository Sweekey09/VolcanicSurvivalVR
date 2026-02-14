using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Data")]
    public EnemyData data;          // ScriptableObject

    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Slider hpSlider;         // World-space HP bar slider

    // Optional: used by WaveManager to know when an enemy dies
    public event Action<EnemyHealth> OnDied;

    private void Awake()
    {
        // Safety: auto-find the slider if you forgot to assign it
        if (hpSlider == null)
            hpSlider = GetComponentInChildren<Slider>(true);
    }

    private void Start()
    {
        if (data != null)
            maxHP = data.maxHP;

        currentHP = maxHP;
        UpdateUI();
    }

    // Called by bullets
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        UpdateUI();

        if (currentHP == 0)
            Die();
    }

    // Called by WaveManager after spawning
    public void SetStats(EnemyData enemyData, int wave)
    {
        data = enemyData;

        // Example scaling: +25% HP per wave (wave 1 = base)
        float hpMultiplier = 1f + 0.25f * (wave - 1);

        maxHP = Mathf.CeilToInt(data.maxHP * hpMultiplier);
        currentHP = maxHP;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f;
            hpSlider.value = (float)currentHP / maxHP;
        }
    }

    private void Die()
    {
        OnDied?.Invoke(this);
        Destroy(gameObject);
    }
}
