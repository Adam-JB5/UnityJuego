using UnityEngine;
using System.Collections;

public class MovimientoPorCasilla : MonoBehaviour
{
    [Header("Propiedades generales")]
    public float tamañoCasilla = 1f;
    public float velocidadMovimiento = 20f;

    [Header("Configuración Swipe")]
    [Tooltip("Distancia mínima en píxeles para que se considere un swipe")]
    public float pixelDistanciaMinima = 50f; 
    private Vector2 inicioToque;
    private Vector2 finToque;

    [Header("Stun")]
    public float tiempoStun = 1f;

    private bool moviendo = false;
    private bool atacando = false;
    private bool aturdido = false;
    private bool castigoPendiente = false;
    private bool movimientoAdelante = false;
    private bool volverAutomaticamente = false;

    private Vector3 destino;
    private Vector3 posicionAnterior;

    private Renderer rend;
    private Color colorOriginal;

    [Header("Animator y efectos")]
    public Animator animator; 
    public GameObject efectoAturdido;
    public GameObject areaAturdido;
    public GameObject efectoAtaqueUsuario;
    public GameObject efectoMuerte;

    void Start()
    {
        destino = transform.position;
        posicionAnterior = transform.position;
        rend = GetComponent<Renderer>();
        colorOriginal = rend.material.color;

        if (efectoAtaqueUsuario != null) efectoAtaqueUsuario.SetActive(false);
        if (efectoAturdido != null) efectoAturdido.SetActive(false);
        if (areaAturdido != null) areaAturdido.SetActive(false);
    }

    void Update()
    {
        if (aturdido) return;

        Vector3 direccion = Vector3.zero;

        // INPUT: Combinación de Teclado y Touch
        if (!moviendo && !atacando && transform.position == destino)
        {
            // 1. Detección por Teclado (Para PC)
            if (Input.GetKeyDown(KeyCode.W)) direccion = Vector3.forward;
            else if (Input.GetKeyDown(KeyCode.A)) direccion = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.D)) direccion = Vector3.right;
            else if (Input.GetKeyDown(KeyCode.S)) direccion = Vector3.back; // Opcional: añadir atrás

            // 2. Detección por Swipe (Para Android)
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                {
                    inicioToque = t.position;
                }
                else if (t.phase == TouchPhase.Ended)
                {
                    finToque = t.position;
                    direccion = CalcularDireccionSwipe();
                }
            }

            // Procesar el movimiento si hay una dirección
            if (direccion != Vector3.zero)
            {
                movimientoAdelante = (direccion == Vector3.forward);
                
                if (movimientoAdelante)
                {
                    volverAutomaticamente = true;
                    atacando = true;
                    animator.SetTrigger("attack2");
                }
                else
                {
                    animator.SetBool("isMoving", true);
                }

                posicionAnterior = transform.position;
                destino = transform.position + direccion * tamañoCasilla;
                moviendo = true;
            }
        }

        // LÓGICA DE MOVIMIENTO VISUAL (Igual que el tuyo)
        Vector3 personajePosicion = animator.transform.position;

        if (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
        }

        if (movimientoAdelante)
            animator.transform.position = personajePosicion;

        if (moviendo && transform.position == destino)
        {
            moviendo = false;
            if (!movimientoAdelante) animator.SetBool("isMoving", false);

            if (movimientoAdelante && volverAutomaticamente)
            {
                destino = posicionAnterior;
                moviendo = true;
                volverAutomaticamente = false;
                return;
            }

            movimientoAdelante = false;
            if (atacando) atacando = false;

            if (castigoPendiente)
            {
                castigoPendiente = false;
                StartCoroutine(Aturdir());
            }
        }
    }

    // Nueva función para procesar el deslizamiento del dedo
    Vector3 CalcularDireccionSwipe()
    {
        Vector2 diferencia = finToque - inicioToque;

        // Verificar si el swipe es lo suficientemente largo
        if (diferencia.magnitude < pixelDistanciaMinima)
            return Vector3.zero;

        // Determinar si fue más horizontal o vertical
        if (Mathf.Abs(diferencia.x) > Mathf.Abs(diferencia.y))
        {
            // Horizontal
            return diferencia.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            // Vertical
            return diferencia.y > 0 ? Vector3.forward : Vector3.back;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // SOLO rebota si el usuario se está moviendo activamente (él intentó pisar al enemigo)
            if (moviendo)
            {
                volverAutomaticamente = false;
                destino = posicionAnterior;
                // No cambiamos moviendo a true porque ya lo está

                if (efectoAtaqueUsuario != null)
                    StartCoroutine(ActivarEfectoConRetardo(efectoAtaqueUsuario, 0.4f));
            }
            // Si el usuario estaba quieto y el enemigo le pegó, NO rebota el usuario.
        }

        if (other.CompareTag("Pared") && !aturdido)
        {
            castigoPendiente = true;
            volverAutomaticamente = false;
            destino = posicionAnterior;
            moviendo = true;

            if (efectoAturdido != null)
                efectoAturdido.SetActive(true);

            if (areaAturdido != null)
                areaAturdido.SetActive(true);
        }
    }

    IEnumerator Aturdir()
    {
        aturdido = true;

        animator.SetBool("isDizzy", true);
        rend.material.color = Color.yellow;

        yield return new WaitForSeconds(tiempoStun);

        if (efectoAturdido != null)
            efectoAturdido.SetActive(false);

        if (areaAturdido != null)
            areaAturdido.SetActive(false);

        rend.material.color = colorOriginal;
        animator.SetBool("isDizzy", false);
        aturdido = false;
    }

    IEnumerator ActivarEfectoConRetardo(GameObject efecto, float delay)
    {
        efecto.SetActive(false);
        yield return null;
        yield return new WaitForSeconds(delay);
        efecto.SetActive(true);
    }

    public bool EsAtacando() { return atacando || moviendo; }
}
