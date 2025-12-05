using UnityEngine;

public class HUD_Manager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUD_Health healthBar;

    private void Start()
    {
        UpdateHealthBar(playerStats.currentHealth);
    }

    private void UpdateHealthBar(float value)
    {
        healthBar.UpdateHealth(value);
    }
}