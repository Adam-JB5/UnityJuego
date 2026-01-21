using UnityEngine;
using System.Collections;

public class MovimientoPorCasilla : MonoBehaviour
{
    public float tamañoCasilla = 1f;
    public float velocidadMovimiento = 20f;

    private bool moviendo = false;
    private bool atacando = false;
    private bool aturdido = false;
    private bool castigoPendiente = false;
    private bool movimientoAdelante = false; // NUEVO: bandera para W
    private Vector3 destino;
    private Vector3 posicionAnterior;
    private Renderer rend;
    private Color colorOriginal;
    public Animator animator; // Animator del personaje hijo
    public GameObject efectoAturdido;
    public GameObject efectoAtaqueEnemigo;
    public GameObject efectoAtaqueUsuario;
    public GameObject efectoMuerte;


    void Start()
    {
        destino = transform.position;
        posicionAnterior = transform.position;

        rend = GetComponent<Renderer>();
        colorOriginal = rend.material.color;
    }

    void Update()
    {
        if (aturdido)
            return;

        Vector3 direccion = Vector3.zero;

        // Input de movimiento
        if (!moviendo && !atacando && transform.position == destino)
        {
            movimientoAdelante = false; // Reset al inicio

            if (Input.GetKeyDown(KeyCode.W))
            {
                direccion = Vector3.forward;
                movimientoAdelante = true; // Activamos flag para W
            }
            if (Input.GetKeyDown(KeyCode.A)) direccion = Vector3.left;
            if (Input.GetKeyDown(KeyCode.D)) direccion = Vector3.right;

            if (direccion != Vector3.zero)
            {
                posicionAnterior = transform.position;
                destino = transform.position + direccion * tamañoCasilla;
                moviendo = true;

                // Animación de caminar solo si es lateral
                if (!movimientoAdelante)
                    animator.SetBool("isMoving", true);
            }
        }

        // Guardamos la posición del personaje antes de mover el cubo
        Vector3 personajePosicion = animator.transform.position;

        // Movimiento del cubo
        if (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadMovimiento * Time.deltaTime
            );
        }

        // Mantener al personaje en su lugar si va hacia adelante
        if (movimientoAdelante)
        {
            animator.transform.position = personajePosicion;
        }

        // Al llegar al destino
        if (moviendo && transform.position == destino)
        {
            moviendo = false;

            // Solo desactivar caminar si era lateral
            if (!movimientoAdelante)
                animator.SetBool("isMoving", false);

            movimientoAdelante = false;

            if (atacando)
                atacando = false;

            if (castigoPendiente)
            {
                castigoPendiente = false;
                StartCoroutine(Aturdir());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !atacando)
        {
            atacando = true;
            destino = posicionAnterior;
            moviendo = true;

            // Animación de ataque del personaje
            animator.SetTrigger("attack2");


            // Activar efecto de golpe con retardo
            if (efectoAtaqueUsuario != null) StartCoroutine(ActivarEfectoConRetardo(efectoAtaqueUsuario, 0.5f));
        }

        if (other.CompareTag("Pared") && !aturdido)
        {
            castigoPendiente = true;
            destino = posicionAnterior;
            moviendo = true;

            // Activar efecto de inmediato al colisionar
            if (efectoAturdido != null) efectoAturdido.SetActive(true);
        }
    }

    IEnumerator Aturdir()
    {
        aturdido = true;

        // Animación de aturdido en el personaje
        animator.SetBool("isDizzy", true);

        // Cambio de color del cubo
        rend.material.color = Color.yellow;

        yield return new WaitForSeconds(1f);

        // Desactivar el efecto
        if (efectoAturdido != null) efectoAturdido.SetActive(false);

        rend.material.color = colorOriginal;
        aturdido = false;

        // Terminar animación de aturdido
        animator.SetBool("isDizzy", false);
    }

    // Coroutine para activar un efecto tras un retardo
    IEnumerator ActivarEfectoConRetardo(GameObject efecto, float delay)
    {
        yield return new WaitForSeconds(delay); // Espera el tiempo deseado
        efecto.SetActive(true);

        // Si quieres que se desactive automáticamente tras un tiempo, puedes hacer:
        // yield return new WaitForSeconds(0.5f); // tiempo activo del efecto
        // efecto.SetActive(false);
    }
}
