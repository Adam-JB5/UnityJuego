using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 1;
    public string targetTag;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DAMAGE] {name} colisiona con {other.name}");

        if (!other.CompareTag(targetTag))
        {
            Debug.Log($"[DAMAGE] Tag incorrecto ({other.tag}), esperado: {targetTag}");
            return;
        }

        Health health = other.GetComponent<Health>();

        if (health == null)
        {
            Debug.Log($"[DAMAGE] {other.name} no tiene Health");
            return;
        }

        Debug.Log($"[DAMAGE] {name} hace {damage} daño a {other.name}");
        health.TakeDamage(damage);
    }
}
