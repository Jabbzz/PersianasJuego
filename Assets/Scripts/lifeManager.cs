using UnityEngine;
using UnityEngine.UI;

public class lifeManager : MonoBehaviour
{
    public static lifeManager instance;

    public int maxLives = 5;
    public int currentLives;

    public Image[] lifeImages;  // Asigna aquí las imágenes en el inspector
    public Sprite fullHeart;    // Sprite para vida completa
    public Sprite emptyHeart;   // Sprite para vida vacía

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentLives = maxLives;
        UpdateUI();
    }

    // lifeManager.cs
    public void LoseLife(bool isPlayer)
    {
        if (isPlayer && currentLives > 0)
        {
            currentLives--;
            UpdateUI();
            if (currentLives <= 0)
            {
                GameOver();
            }
        }
    }

  

    private void GameOver()
    {
        Debug.Log("¡Game Over! Reiniciando jugador...");
        FindObjectOfType<Playercontroller>()?.ResetPlayer();
        // Puedes recargar la escena, ir a una pantalla de fin, etc.
        // SceneManager.LoadScene("GameOverScene");
    }


    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (i < currentLives)
            {
                lifeImages[i].sprite = fullHeart;
            }
            else
            {
                lifeImages[i].sprite = emptyHeart;
            }
        }
    }
}
