using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    public static event System.Action<float> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    void Update()
    {
        
        
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        NotifyHealthChanged();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        float normalized = currentHealth / maxHealth; // 0 to 1
        OnHealthChanged?.Invoke(normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //get bullet damage value and remove it from hp
        }
    }
}