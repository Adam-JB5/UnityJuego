using UnityEngine;

public class MusicaPersistente : MonoBehaviour
{
    void Awake()
    {
        // Esta es la instrucción mágica:
        // Le dice a Unity que este objeto NO se destruya al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
    }
}