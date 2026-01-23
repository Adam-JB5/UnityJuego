using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public bool isDead { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        Debug.Log($"[HEALTH] {name} inicia con {currentHealth}/{maxHealth} HP");
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            Debug.Log($"[HEALTH] {name} ya está muerto. Daño ignorado.");
            return;
        }

        currentHealth -= amount;

        Debug.Log($"[HEALTH] {name} recibe {amount} daño → {currentHealth}/{maxHealth} HP");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log($"[HEALTH] {name} HA MUERTO ☠️");

        // Aviso a otros scripts (animación, drop, etc.)
        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    }
}
