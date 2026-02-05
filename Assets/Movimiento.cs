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
    private bool movimientoAdelante = false;
    private bool volverAutomaticamente = false;

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

        if (efectoAtaqueUsuario != null)
            efectoAtaqueUsuario.SetActive(false);

        if (efectoAturdido != null)
            efectoAturdido.SetActive(false);
    }

    void Update()
    {
        if (aturdido)
            return;

        Vector3 direccion = Vector3.zero;

        // INPUT
        if (!moviendo && !atacando && transform.position == destino)
        {
            movimientoAdelante = false;

            if (Input.GetKeyDown(KeyCode.W))
            {
                direccion = Vector3.forward;
                movimientoAdelante = true;
                volverAutomaticamente = true;

                // ATAQUE SIEMPRE
                atacando = true;
                animator.SetTrigger("attack2");

                
            }

            if (Input.GetKeyDown(KeyCode.A))
                direccion = Vector3.left;

            if (Input.GetKeyDown(KeyCode.D))
                direccion = Vector3.right;

            if (direccion != Vector3.zero)
            {
                posicionAnterior = transform.position;
                destino = transform.position + direccion * tamañoCasilla;
                moviendo = true;

                if (!movimientoAdelante)
                    animator.SetBool("isMoving", true);
            }
        }

        // Guardamos posición visual del personaje
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

        // El personaje no se desplaza al atacar
        if (movimientoAdelante)
            animator.transform.position = personajePosicion;

        // LLEGADA
        if (moviendo && transform.position == destino)
        {
            moviendo = false;

            if (!movimientoAdelante)
                animator.SetBool("isMoving", false);

            // Vuelta automática si no pasó nada
            if (movimientoAdelante && volverAutomaticamente)
            {
                destino = posicionAnterior;
                moviendo = true;
                volverAutomaticamente = false;
                return;
            }

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
        if (other.CompareTag("Enemy"))
    {
        // SOLO rebota si el usuario se está moviendo activamente (él intentó pisar al enemigo)
        if (moviendo) 
        {
            volverAutomaticamente = false;
            destino = posicionAnterior;
            // No cambiamos moviendo a true porque ya lo está
            
            if (efectoAtaqueUsuario != null)
                StartCoroutine(ActivarEfectoConRetardo(efectoAtaqueUsuario, 0.52f));
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
        }
    }

    IEnumerator Aturdir()
    {
        aturdido = true;

        animator.SetBool("isDizzy", true);
        rend.material.color = Color.yellow;

        yield return new WaitForSeconds(1f);

        if (efectoAturdido != null)
            efectoAturdido.SetActive(false);

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
