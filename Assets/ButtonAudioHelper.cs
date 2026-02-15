using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar el mouse

public class ButtonAudioHelper : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private MainMenu mainMenu;

    void Start()
    {
        // Busca el script principal en la escena
        mainMenu = Object.FindFirstObjectByType<MainMenu>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mainMenu.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mainMenu.PlayClick();
    }
}