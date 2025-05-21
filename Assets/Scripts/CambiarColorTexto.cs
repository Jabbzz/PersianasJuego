using UnityEngine;
using TMPro;

public class CambiarColorTexto : MonoBehaviour
{
    public TextMeshProUGUI texto;

    public void CambiarColor()
    {
        texto.color = Color.red; // Cambia a cualquier color que quieras
    }
}
