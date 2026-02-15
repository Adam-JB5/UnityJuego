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

    [Header("Efectos")]
    public ShakeUI shakeScript;
    public GameObject efectoMuerteFinal; // <--- NUEVO: Arrastra aquí tu efecto épico

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoImpacto;
    public AudioClip sonidoMuerte;

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

        // --- SONIDO DE IMPACTO ---
        if (audioSource != null && sonidoImpacto != null)
            audioSource.PlayOneShot(sonidoImpacto, 1.2f);

        // 1. DISPARAR ANIMACIÓN DE GOLPE
        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("hit");
        }

        // 2. Activa el Shake si la referencia existe
        if (shakeScript != null)
        {
            shakeScript.Shake();
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

        // --- SONIDO DE MUERTE ---
        if (audioSource != null && sonidoMuerte != null)
            audioSource.PlayOneShot(sonidoMuerte, 1.2f);

        // LÓGICA DE ACTIVAR OBJETO EXISTENTE
        if (efectoMuerteFinal != null)
        {
            // En lugar de crear uno nuevo, simplemente activamos el que ya existe
            efectoMuerteFinal.SetActive(true);

            // Si el efecto es hijo del enemigo y el enemigo se desactiva,
            // el efecto también se apagará. Si quieres que siga brillando,
            // tenemos que "sacarlo" del padre:
            efectoMuerteFinal.transform.SetParent(null);
        }

        if (animator != null) animator.SetBool("isDead", true);

        var movimiento = GetComponent<EnemigoModularInteligente>();
        if (movimiento != null) movimiento.enabled = false;

        StartCoroutine(SecuenciaVictoriaDramatica());
    }

    IEnumerator SecuenciaVictoriaDramatica()
    {
        // Iniciamos la cámara lenta
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Esperamos 2 segundos en tiempo REAL (porque el tiempo del juego está casi parado)
        yield return new WaitForSecondsRealtime(2f);

        // Restauramos el tiempo
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // MOSTRAR EL MENÚ
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