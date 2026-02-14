using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Slider hpSlider;

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
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        UpdateUI();

        if (currentHP == 0)
        {
            Debug.Log("Player died!");
        }
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHP; 
    }
}