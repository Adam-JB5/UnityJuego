using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void PlayGame() => SceneManager.LoadScene("GameScene");

    public void QuitGame()
    {
        // Si estamos en el Editor de Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si es el juego final (Build)
            Application.Quit();
        #endif
    }

    // Funciones que llamaremos desde los botones
    public void PlayHover() { if (hoverSound) audioSource.PlayOneShot(hoverSound, 0.7f); }
    public void PlayClick() { if (clickSound) audioSource.PlayOneShot(clickSound, 1.2f); }
}