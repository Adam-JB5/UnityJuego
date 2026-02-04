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

    [Header("Ataque")]
    public bool puedeAtacar = true;
    public float distanciaDeteccion = 1.5f; // Un poco más de 1 casilla
    private Transform jugador;

    [ContextMenu("Cambiar a Modo Aleatorio")]
    void SetAleatorio() { modoActual = ModoMovimiento.Aleatorio; }

    [ContextMenu("Cambiar a Modo Lineal")]
    void SetLineal() { modoActual = ModoMovimiento.Lineal; }

    void Start()
    {
        posicionInicial = transform.position;
        destino = transform.position;
        
        // Buscamos al jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("User");
        if (playerObj != null) jugador = playerObj.transform;

        StartCoroutine(RutinaMovimiento());
    }

    void Update()
    {
        // Solo manejamos el desplazamiento visual si no estamos atacando (la corrutina de ataque ya mueve el transform)
        ManejarDesplazamientoVisual();
    }

    IEnumerator RutinaMovimiento()
    {
        // Comprobamos la salud para dejar de movernos si morimos
        Health healthComponent = GetComponent<Health>();

        while (healthComponent != null && !healthComponent.isDead)
        {
            yield return new WaitForSeconds(tiempoEntrePasos);

            if (!moviendo)
            {
                if (jugador != null)
                {
                    float distancia = Vector3.Distance(transform.position, jugador.position);

                    if (puedeAtacar && distancia <= distanciaDeteccion)
                    {
                        Vector3 dirAlJugador = (jugador.position - transform.position).normalized;
                        StartCoroutine(EjecutarAtaqueVisual(dirAlJugador));
                        continue; // Saltamos el resto del bucle para no movernos después de atacar
                    }
                }

                // Si no atacó, patrulla
                if (modoActual == ModoMovimiento.Lineal)
                    CalcularSiguientePasoLineal();
                else
                    CalcularSiguientePasoAleatorio();
            }
        }
    }

    // ESTA ES LA FUNCIÓN QUE TE FALTABA
    IEnumerator EjecutarAtaqueVisual(Vector3 dirAtaque)
    {
        moviendo = true;
        if (animator != null) animator.SetTrigger("attack"); 

        Vector3 posOriginal = transform.position;
        // Calculamos un punto medio hacia el jugador para la embestida
        Vector3 puntoImpacto = transform.position + dirAtaque * (tamañoCasilla * 0.5f);

        // Embestida hacia adelante
        float t = 0;
        while(t < 1) {
            t += Time.deltaTime * 15f; 
            transform.position = Vector3.Lerp(posOriginal, puntoImpacto, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f); // Breve pausa en el impacto

        // Regreso a la casilla original
        t = 0;
        while(t < 1) {
            t += Time.deltaTime * 10f;
            transform.position = Vector3.Lerp(puntoImpacto, posOriginal, t);
            yield return null;
        }

        transform.position = posOriginal;
        destino = posOriginal; // Actualizamos destino para que ManejarDesplazamientoVisual no intente moverlo
        moviendo = false;
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
        pasoActual += direccion;
        destino = posicionInicial + (Vector3.right * (pasoActual * tamañoCasilla));
        moviendo = true;
        if (animator != null) animator.SetBool("isMoving", true);
    }

    void ManejarDesplazamientoVisual()
    {
        // Solo movemos si el destino es diferente y no estamos en medio de la corrutina de ataque
        if (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
        }
        else if (moviendo)
        {
            // Si el enemigo está en el destino pero aún marcaba "moviendo", lo paramos
            // (a menos que sea el ataque, que se gestiona solo)
            if (Vector3.Distance(transform.position, destino) < 0.01f)
            {
                 // No reseteamos 'moviendo' aquí si se está ejecutando el ataque
            }
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