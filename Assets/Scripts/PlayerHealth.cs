using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Slider hpSlider;

    [Header("Lose Scene")]
    public LoseLoader loseLoader;   // drag GameManager (with LoseLoader) here
    private bool isDead = false;

    private void Start()
    {
        currentHP = maxHP;

        // Make slider match HP system
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHP;
        }

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        UpdateUI();

        if (currentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        if (loseLoader != null)
            loseLoader.LoadLoseScene();
        else
            Debug.LogError("LoseLoader not assigned in PlayerHealth. Drag GameManager into it!");
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHP;
    }
}
