using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public bool isDead { get; private set; }

    [Header("Referencias")]
    public Animator animator;
    public GameObject canvasVictoryMenu;

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
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetBool("isDead", true);

        // Detener movimiento
        var movimiento = GetComponent<EnemigoModularInteligente>();
        if (movimiento != null) movimiento.enabled = false;

        // Lanzar la secuencia de victoria
        StartCoroutine(SecuenciaVictoriaDramatica());
    }

    IEnumerator SecuenciaVictoriaDramatica()
    {
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // 4. MOSTRAR EL MENÚ (Usando la referencia directa)
        if (canvasVictoryMenu != null)
        {
            canvasVictoryMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogError("¡No has arrastrado el Canvas al script del enemigo!");
        }
    }
}