using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VidaEnemigoFlotante : MonoBehaviour
{
    [Header("Referencias")]
    public Health scriptSalud;       // El script Health del enemigo
    public GameObject prefabCorazon; // El mismo prefab que usaste para el jugador
    public Transform contenedor;     // Un objeto vacío hijo del enemigo con un Horizontal Layout Group

    [Header("Ajustes")]
    public Sprite corazonLleno;
    public Sprite corazonVacio;

    private List<Image> imagenesCorazones = new List<Image>();
    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        if (scriptSalud != null) GenerarCorazones();
    }

    void Update()
    {
        ActualizarCorazones();
        HacerQueMireALaCamara();
    }

    void GenerarCorazones()
    {
        foreach (Transform hijo in contenedor) Destroy(hijo.gameObject);
        imagenesCorazones.Clear();

        for (int i = 0; i < scriptSalud.maxHealth; i++)
        {
            GameObject nuevo = Instantiate(prefabCorazon, contenedor);
            // Reducimos un poco la escala si es para el enemigo
            nuevo.transform.localScale = Vector3.one * 0.8f; 
            imagenesCorazones.Add(nuevo.GetComponent<Image>());
        }
    }

    void ActualizarCorazones()
    {
        for (int i = 0; i < imagenesCorazones.Count; i++)
        {
            imagenesCorazones[i].sprite = (i < scriptSalud.currentHealth) ? corazonLleno : corazonVacio;
        }
    }

    void HacerQueMireALaCamara()
    {
        // Esto hace que los corazones siempre miren al jugador
        contenedor.parent.LookAt(contenedor.parent.position + camaraPrincipal.transform.rotation * Vector3.forward,
                                 camaraPrincipal.transform.rotation * Vector3.up);
    }
}