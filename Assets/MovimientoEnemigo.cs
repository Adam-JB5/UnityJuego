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

    private Vector3 posicionInicial;
    private Vector3 destino;
    private bool moviendo = false;
    private int pasoActual = 0;
    private int direccionLineal = -1; // -1 izq, 1 der

    public Animator animator;

    // BOTONES EN EL INSPECTOR (Click derecho en el componente)
    [ContextMenu("Cambiar a Modo Aleatorio")]
    void SetAleatorio() { modoActual = ModoMovimiento.Aleatorio; }

    [ContextMenu("Cambiar a Modo Lineal")]
    void SetLineal() { modoActual = ModoMovimiento.Lineal; }

    void Start()
    {
        posicionInicial = transform.position;
        destino = transform.position;
        StartCoroutine(RutinaMovimiento());
    }

    void Update()
    {
        ManejarDesplazamientoVisual();
    }

    IEnumerator RutinaMovimiento()
    {
        while (!GetComponent<Health>().isDead) // <--- Solo se mueve si NO está muerto
        {
            yield return new WaitForSeconds(tiempoEntrePasos);
            while (true)
            {
                yield return new WaitForSeconds(tiempoEntrePasos);
                if (!moviendo)
                {
                    if (modoActual == ModoMovimiento.Lineal)
                        CalcularSiguientePasoLineal();
                    else
                        CalcularSiguientePasoAleatorio();
                }
            }
        }
    }

    void CalcularSiguientePasoLineal()
    {
        // Lógica de rebote
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
        pasoActual += direccion;
        destino = posicionInicial + (Vector3.right * (pasoActual * tamañoCasilla));
        moviendo = true;
        if (animator != null) animator.SetBool("isMoving", true);
    }

    void ManejarDesplazamientoVisual()
    {
        if (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
        }
        else if (moviendo)
        {
            moviendo = false;
            if (animator != null) animator.SetBool("isMoving", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = (modoActual == ModoMovimiento.Lineal) ? Color.cyan : Color.magenta;
        Vector3 inicio = (Application.isPlaying) ? posicionInicial : transform.position;
        Vector3 pIzq = inicio + (Vector3.left * (limiteIzquierda * tamañoCasilla));
        Vector3 pDer = inicio + (Vector3.right * (limiteDerecha * tamañoCasilla));
        Gizmos.DrawLine(pIzq, pDer);
    }
}