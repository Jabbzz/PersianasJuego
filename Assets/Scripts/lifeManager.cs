using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class lifeManager : MonoBehaviour
{
    public static lifeManager instance;

    public int maxLives = 5;
    public int currentLives;

    public Image[] lifeImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Vector3 initialPosition;

    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }

    private void Start()
    {
        initialPosition = GameObject.FindWithTag("Player").transform.position;
    }

    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reasignar referencias de UI al cargar nueva escena
        ReassignUIReferences();
        InitializeLives();
        if (SceneManager.GetActiveScene().name == "Creditos")
        {
            Destroy(gameObject);
        }
    }

    private void ReassignUIReferences()
    {
        // Buscar y asignar las imágenes de vida en la nueva escena
        var lifeImagesParent = GameObject.Find("LifeImagesPanel");
        if (lifeImagesParent != null)
        {
            lifeImages = lifeImagesParent.GetComponentsInChildren<Image>();
        }
    }

    public void InitializeLives()
    {
        currentLives = maxLives;
        UpdateUI();
    }

    public void LoseLife(bool isPlayer)
    {
        if (!isPlayer) return;

        if (currentLives > 0)
        {
            currentLives--;
            UpdateUI();

            if (currentLives <= 0)
            {
                HandleGameOver();
            }
        }
    }

    private void HandleGameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateUI()
    {
        if (lifeImages == null) return;

        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (lifeImages[i] != null)
            {
                lifeImages[i].sprite = i < currentLives ? fullHeart : emptyHeart;
                lifeImages[i].enabled = i < maxLives;
            }
        }
    }
}