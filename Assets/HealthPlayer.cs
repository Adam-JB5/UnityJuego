using UnityEngine;
using System.Collections;

public class HealthJugador : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public bool isDead { get; private set; }

    [Header("Referencias")]
    public Animator animator;
    public GameObject canvasGameOver; // <--- Cambiado para el jugador

    void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Jugador recibe daño. Vida restante: " + currentHealth);

        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("hit"); // Asegúrate de que tu animador tenga este trigger
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

        // 1. Lanzar animación de muerte
        if (animator != null) animator.SetBool("isDead", true);

        // 2. DESACTIVAR TU MOVIMIENTO (Para que no puedas seguir moviéndote muerto)
        MovimientoPorCasilla mov = GetComponent<MovimientoPorCasilla>();
        if (mov != null) mov.enabled = false;

        // 3. Mostrar el menú
        StartCoroutine(SecuenciaMuerte());
    }

    IEnumerator SecuenciaMuerte()
    {
        // Un poco de cámara lenta para sentir el golpe
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1f;

        if (canvasGameOver != null)
        {
            canvasGameOver.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}