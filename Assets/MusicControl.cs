using UnityEngine;

public class MusicControl : MonoBehaviour
{
    private static MusicControl instance;

    void Awake()
    {
        // Si ya existe una instancia de este objeto, destruye la nueva para no duplicar
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Si es la primera vez, asígnala y dile que no se destruya
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}