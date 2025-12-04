using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event System.Action<float> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    void Update()
    {
        // TEST: Damage
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("HIT! Before damage: " + currentHealth);
            TakeDamage(10f);
            Debug.Log("After damage: " + currentHealth);
        }

        // TEST: Heal
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("HEAL! Before heal: " + currentHealth);
            Heal(10f);
            Debug.Log("After heal: " + currentHealth);
        }
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