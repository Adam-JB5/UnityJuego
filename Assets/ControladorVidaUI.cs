using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControladorVidaUI : MonoBehaviour
{
    [Header("Referencias")]
    public HealthJugador healthJugador; // Arrastra aquí al Jugador
    public GameObject prefabCorazon;   // Un Image con el dibujo del corazón

    [Header("Ajustes Visuales")]
    public Sprite corazonLleno;
    public Sprite corazonVacio;

    private List<Image> imagenesCorazones = new List<Image>();

    void Start()
    {
        if (healthJugador != null)
        {
            GenerarCorazones();
        }
    }

    void Update()
    {
        ActualizarInterfaz();
    }

    // Crea tantos corazones como maxHealth tenga el jugador
    void GenerarCorazones()
    {
        // Limpiamos por si acaso
        foreach (Transform hijo in transform) Destroy(hijo.gameObject);
        imagenesCorazones.Clear();

        for (int i = 0; i < healthJugador.maxHealth; i++)
        {
            GameObject nuevoCorazon = Instantiate(prefabCorazon, transform);
            Image img = nuevoCorazon.GetComponent<Image>();
            img.sprite = corazonLleno;
            imagenesCorazones.Add(img);
        }
    }

    // Cambia el sprite según la vida actual
    void ActualizarInterfaz()
    {
        for (int i = 0; i < imagenesCorazones.Count; i++)
        {
            if (i < healthJugador.currentHealth)
            {
                imagenesCorazones[i].sprite = corazonLleno;
                imagenesCorazones[i].color = Color.white; // Opción: que brillen
            }
            else
            {
                imagenesCorazones[i].sprite = corazonVacio;
                imagenesCorazones[i].color = new Color(1, 1, 1, 0.5f); // Opción: más transparentes
            }
        }
    }
}