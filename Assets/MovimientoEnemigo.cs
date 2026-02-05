using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemigoModularInteligente : MonoBehaviour
{
    public enum ModoMovimiento { Lineal, Aleatorio }

    [Header("Configuración de IA")]
    public ModoMovimiento modoActual = ModoMovimiento.Lineal;

    [Header("Configuración de Movimiento")]
    public float tamañoCasilla = 1f;
    public float velocidadMovimiento = 20f;
    public float tiempoEntrePasos = 1.5f;

    [Header("Rango de Patrulla")]
    public int limiteIzquierda = 4;
    public int limiteDerecha = 0;

    [Header("Probabilidad de Ataque")]
    [Range(0, 100)]
    public int probabilidadAtaque = 30;
    public Vector3 direccionAtaque = Vector3.forward;

    private Vector3 posicionInicial;
    private Vector3 destino;
    private Vector3 posicionAnterior;
    private bool moviendo = false;
    private bool atacando = false;
    private int pasoActual = 0;
    private int direccionLineal = -1;

    public Animator animator;
    public GameObject efectoAtaqueEnemigo;

    void Start()
    {
        posicionInicial = transform.position;
        destino = transform.position;
        posicionAnterior = transform.position;
        StartCoroutine(RutinaMovimiento());
    }

    void Update()
    {
        // El motor de movimiento siempre corre, igual que en tu usuario
        ManejarDesplazamientoVisual();
    }

    IEnumerator RutinaMovimiento()
    {
        Health healthComponent = GetComponent<Health>();

        while (healthComponent != null && !healthComponent.isDead)
        {
            yield return new WaitForSeconds(tiempoEntrePasos);

            if (!moviendo && !atacando)
            {
                int dado = Random.Range(0, 101);

                if (dado <= probabilidadAtaque)
                {
                    StartCoroutine(AtacarComoUsuario());
                }
                else
                {
                    if (modoActual == ModoMovimiento.Lineal)
                        CalcularSiguientePasoLineal();
                    else
                        CalcularSiguientePasoAleatorio();
                }
            }
        }
    }

    // LÓGICA DE ATAQUE IGUAL A LA DEL USUARIO (W)
    IEnumerator AtacarComoUsuario()
    {
        atacando = true;
        posicionAnterior = transform.position;

        // Seteamos el destino hacia adelante (como el W del usuario)
        destino = transform.position + direccionAtaque * tamañoCasilla;
        moviendo = true;

        if (animator != null) animator.SetTrigger("attack"); // Ajusta el nombre si es attack2

        // Esperamos a que llegue al destino (el impacto)
        yield return new WaitUntil(() => transform.position == destino);

        // Pequeña pausa de impacto (opcional)
        yield return new WaitForSeconds(0.1f);

        // Volver automáticamente (como tu "volverAutomaticamente = true")
        destino = posicionAnterior;
        moviendo = true;

        // Esperamos a que vuelva
        yield return new WaitUntil(() => transform.position == destino);

        atacando = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("User") || other.CompareTag("Player"))
        {
            // El enemigo siempre vuelve a su casilla al tocar al jugador
            destino = posicionAnterior;
            moviendo = true;

            if (efectoAtaqueEnemigo != null)
            {
                efectoAtaqueEnemigo.SetActive(false);
                efectoAtaqueEnemigo.SetActive(true);
            }

            // Opcional: Si quieres detener la corrutina de ataque para que no intente seguir avanzando
            StopCoroutine("AtacarComoUsuario");
            atacando = false;
        }
    }

    void CalcularSiguientePasoLineal()
    {
        if (direccionLineal == -1 && pasoActual <= -limiteIzquierda) direccionLineal = 1;
        else if (direccionLineal == 1 && pasoActual >= limiteDerecha) direccionLineal = -1;

        EjecutarMovimiento(direccionLineal);
    }

    void CalcularSiguientePasoAleatorio()
    {
        List<int> opciones = new List<int>();
        if (pasoActual > -limiteIzquierda) opciones.Add(-1);
        if (pasoActual < limiteDerecha) opciones.Add(1);

        if (opciones.Count > 0)
        {
            int eleccion = opciones[Random.Range(0, opciones.Count)];
            EjecutarMovimiento(eleccion);
        }
    }

    void EjecutarMovimiento(int direccion)
    {
        posicionAnterior = transform.position;
        pasoActual += direccion;
        // Importante: Usamos Vector3.right para la patrulla lateral
        destino = posicionInicial + (Vector3.right * (pasoActual * tamañoCasilla));
        moviendo = true;

        if (animator != null) animator.SetBool("isMoving", true);
    }

    void ManejarDesplazamientoVisual()
    {
        if (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadMovimiento * Time.deltaTime
            );
        }
        else if (moviendo)
        {
            moviendo = false;
            if (animator != null) animator.SetBool("isMoving", false);
        }
    }

    public bool EsAtacando()
    {
        // El enemigo hace daño si se está moviendo a una casilla 
        // O si está en medio de la corrutina de ataque.
        return moviendo || atacando;
    }
}