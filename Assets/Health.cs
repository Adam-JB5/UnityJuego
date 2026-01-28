using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public bool isDead { get; private set; }

    [Header("Referencias")]
    public Animator animator; // Arrastra el Animator aquí

    void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        // Si no asignaste el animator manualmente, intenta buscarlo
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // 1. DISPARAR ANIMACIÓN DE GOLPE (Hurt/GetHit)
        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("hit");
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Seguridad extra
        isDead = true;

        if (animator != null)
        {
            // Forzamos el estado de muerte
            animator.SetBool("isDead", true);

            // Opcional: Si tienes otras capas de animación (como una de ataque), 
            // pon su peso en 0 para que no interfieran con la muerte.
            // animator.SetLayerWeight(1, 0f); 
        }

        // Detenemos todas las corrutinas de este objeto (como la de movimiento)
        StopAllCoroutines();

        // Desactivamos el script de movimiento específicamente
        var movimiento = GetComponent<EnemigoModularInteligente>();
        if (movimiento != null) movimiento.enabled = false;

        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    }
}