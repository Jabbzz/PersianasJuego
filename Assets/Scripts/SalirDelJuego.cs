using UnityEngine;

public class SalirDelJuego : MonoBehaviour
{
    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego..."); // Esto solo se verá en el editor
    }
}
