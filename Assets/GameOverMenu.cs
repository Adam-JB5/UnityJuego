using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    // Función para el botón de Reintentar Nivel
    public void RetryLevel()
    {
        Time.timeScale = 1f; // Reset del tiempo por si hubo cámara lenta
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // Función para el botón de Salir al Menú Principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene"); // <-- ESCRIBE AQUÍ EL NOMBRE DE TU MENÚ
    }
}
