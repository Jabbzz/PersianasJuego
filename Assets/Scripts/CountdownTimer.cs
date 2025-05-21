using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;



public class CountdownTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timeAddedText;
    public float timeAddedDisplayDuration = 1.5f;
    
    [Header("Timer Settings")]
    public float initialTime = 180f;
    public float warningThreshold = 30f;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    
    private float currentTime;
    private bool isRunning = true;
    private float timeAddedDisplayTimer;
    private bool isDisplayingTimeAdded;

    private void Awake()
    {
        // Método actualizado para encontrar objetos del mismo tipo
        var existingTimers = FindObjectsByType<CountdownTimer>(FindObjectsSortMode.None);
        if (existingTimers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTimer();
        FindUIReferences();
    }

    private void FindUIReferences()
    {
        if (timerText == null)
        {
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
        }
        if (timeAddedText == null)
        {
            timeAddedText = GameObject.Find("TimeAddedText")?.GetComponent<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        currentTime = initialTime;
        isRunning = true;
        UpdateDisplay();
        
        if (timeAddedText != null)
        {
            timeAddedText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                HandleTimeExpired();
            }

            UpdateDisplay();
        }

        HandleTimeAddedDisplay();
    }

    private void HandleTimeAddedDisplay()
    {
        if (isDisplayingTimeAdded)
        {
            timeAddedDisplayTimer -= Time.deltaTime;
            if (timeAddedDisplayTimer <= 0)
            {
                timeAddedText?.gameObject.SetActive(false);
                isDisplayingTimeAdded = false;
            }
        }
    }

    public void ResetTimer()
    {
        currentTime = initialTime;
        isRunning = true;
        UpdateDisplay();
    }

    private void UpdateDisplay()
{
    if (timerText == null) return;
    
    int minutes = Mathf.FloorToInt(currentTime / 60);
    int seconds = Mathf.FloorToInt(currentTime % 60);
    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    
    if (currentTime <= warningThreshold && currentTime > 0)
    {
        timerText.color = warningColor;
        // Opcional: Mostrar mensaje adicional
        if (!isDisplayingTimeAdded) // Para no superponer con otros mensajes
        {
            timeAddedText.text = "¡Últimos 30 segundos!";
            timeAddedText.gameObject.SetActive(true);
            timeAddedDisplayTimer = 2f; // Mostrar por 2 segundos
            isDisplayingTimeAdded = true;
        }
    }
    else
    {
        timerText.color = normalColor;
    }
}

    private void HandleTimeExpired()
    {
        // Mostrar mensaje de tiempo agotado (opcional)
        if (timeAddedText != null)
        {
            timeAddedText.text = "¡Tiempo agotado!";
            timeAddedText.gameObject.SetActive(true);
            timeAddedDisplayTimer = 2f;
            isDisplayingTimeAdded = true;
        }

        // Reiniciar la escena actual después de un pequeño delay
        StartCoroutine(ReloadSceneAfterDelay(1.5f));
    }

private IEnumerator ReloadSceneAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}


    public void OnPlayerDeath()
    {
        ResetTimer();
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
        
        if (timeAddedText != null)
        {
            timeAddedText.text = $"+{seconds} sec";
            timeAddedText.gameObject.SetActive(true);
            timeAddedDisplayTimer = timeAddedDisplayDuration;
            isDisplayingTimeAdded = true;
        }
    }
}