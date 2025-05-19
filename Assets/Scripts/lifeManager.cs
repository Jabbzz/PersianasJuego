using UnityEngine;
using UnityEngine.UI;

public class lifeManager : MonoBehaviour
{
    public static lifeManager instance;

    public int maxLives = 4;
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

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            UpdateUI();
        }
    }

    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateUI();
        }
    }

    private void UpdateUI()
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
