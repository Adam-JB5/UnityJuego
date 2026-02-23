using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{

    public string siguienteNivel;
    // Función para el botón de Siguiente Nivel
    public void NextLevel()
    {
        Time.timeScale = 1f; // Reset del tiempo por si hubo cámara lenta
        SceneManager.LoadScene(siguienteNivel);
    }

    // Función para el botón de Salir al Menú Principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

}