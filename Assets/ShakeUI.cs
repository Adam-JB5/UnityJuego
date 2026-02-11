using UnityEngine;
using System.Collections;

public class ShakeUI : MonoBehaviour
{
    private Vector3 posicionOriginal;
    public float duracion = 0.2f;
    public float fuerza = 5f;

    void Awake()
    {
        posicionOriginal = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines(); // Por si recibe otro golpe rápido
        StartCoroutine(HacerShake());
    }

    IEnumerator HacerShake()
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < duracion)
        {
            // Genera un movimiento aleatorio
            float x = Random.Range(-1f, 1f) * fuerza;
            float y = Random.Range(-1f, 1f) * fuerza;

            transform.localPosition = new Vector3(posicionOriginal.x + x, posicionOriginal.y + y, posicionOriginal.z);

            tiempoPasado += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        transform.localPosition = posicionOriginal; // Vuelve a su sitio
    }
}