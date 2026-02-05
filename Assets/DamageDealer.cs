using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 1;
    public string targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // 1. OBTENEMOS REFERENCIAS DE QUIÉN SOY YO Y QUIÉN ES EL OTRO
        var miScriptEnemigo = GetComponent<EnemigoModularInteligente>();
        var miScriptJugador = GetComponent<MovimientoPorCasilla>();
        
        var otroScriptEnemigo = other.GetComponent<EnemigoModularInteligente>();
        var otroScriptJugador = other.GetComponent<MovimientoPorCasilla>();

        // 2. LÓGICA DE PRIORIDAD (VENTAJA JUGADOR)
        
        // Caso A: Si YO soy el JUGADOR y estoy moviéndome/atacando
        if (miScriptJugador != null && miScriptJugador.EsAtacando())
        {
            // El jugador SIEMPRE hace daño si está atacando, 
            // ignoramos si el enemigo también estaba atacando.
            AplicarDaño(other);
            return;
        }

        // Caso B: Si YO soy el ENEMIGO y estoy atacando
        if (miScriptEnemigo != null && miScriptEnemigo.EsAtacando())
        {
            // El enemigo SOLO hace daño si el jugador NO está atacando.
            // Si el jugador está atacando, el jugador tiene "escudo" y gana él.
            bool jugadorAtacando = (otroScriptJugador != null && otroScriptJugador.EsAtacando());

            if (!jugadorAtacando)
            {
                AplicarDaño(other);
            }
            else
            {
                Debug.Log("[COMBATE] El jugador bloqueó el ataque del enemigo atacando también.");
            }
            return;
        }
    }

    void AplicarDaño(Collider target)
    {
        // Daño a Jugador
        HealthJugador hpJ = target.GetComponent<HealthJugador>();
        if (hpJ != null)
        {
            hpJ.TakeDamage(damage);
            Debug.Log($"[DAÑO] {name} hirió al Jugador");
            return;
        }

        // Daño a Enemigo
        Health hpE = target.GetComponent<Health>();
        if (hpE != null)
        {
            hpE.TakeDamage(damage);
            Debug.Log($"[DAÑO] {name} hirió al Enemigo");
            return;
        }
    }
}