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

    private Vector3 destino;
    private Vector3 posicionAnterior;

    private Renderer rend;
    private Color colorOriginal;
    public Animator animator;

    void Start()
    {
        destino = transform.position;
        posicionAnterior = transform.position;

        rend = GetComponent<Renderer>();
        colorOriginal = rend.material.color;

        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (aturdido)
            return;

        // Movimiento suave
        if (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadMovimiento * Time.deltaTime
            );
        }

        // Input
        if (!moviendo && !atacando && transform.position == destino)
        {
            Vector3 direccion = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W)) direccion = Vector3.forward;
            if (Input.GetKeyDown(KeyCode.S)) direccion = Vector3.back;
            if (Input.GetKeyDown(KeyCode.A)) direccion = Vector3.left;
            if (Input.GetKeyDown(KeyCode.D)) direccion = Vector3.right;

            if (direccion != Vector3.zero)
            {
                posicionAnterior = transform.position;
                destino = transform.position + direccion * tamañoCasilla;
                moviendo = true;

                // Animación de caminar
                animator.SetBool("isMoving", true);
            }
        }

        // Al llegar al destino
        if (moviendo && transform.position == destino)
        {
            moviendo = false;

            animator.SetBool("isMoving", false);

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

            //Trigger de ataque
            animator.SetTrigger("attack");
        }

        if (other.CompareTag("Pared") && !aturdido)
        {
            castigoPendiente = true;
            destino = posicionAnterior;
            moviendo = true;
        }
    }

    IEnumerator Aturdir()
    {
        aturdido = true;

        // Activar animación de aturdido
        animator.SetBool("isDizzy", true);

        // CAMBIO DE COLOR
        rend.material.color = Color.yellow;

        yield return new WaitForSeconds(1f);

        // VOLVER AL COLOR ORIGINAL
        rend.material.color = colorOriginal;
        aturdido = false;

        // Acabar animación de aturdido
        animator.SetBool("isDizzy", false);
    }
}
