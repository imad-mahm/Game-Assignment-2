using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public static System.Action<float> OnHealthChanged;
    [SerializeField] private float TimeToRegen;
    private float regentimer;
    private bool low;

    public float getPlayerHealth()
    {
        return currentHealth;
    }
    
    
    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        regentimer += Time.deltaTime;

        if (regentimer >= TimeToRegen && low)
        {
            Regenerate();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            low = true;
            regentimer = 0;
            TakeDamage(0f );//change to damage number
        }
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void Regenerate()
    {
        currentHealth += 8;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            low=false;
            return;
        }
        StartCoroutine(wait1());
    }

    private IEnumerator wait1()
    {
        yield return new WaitForSeconds(1f);
    }
}
