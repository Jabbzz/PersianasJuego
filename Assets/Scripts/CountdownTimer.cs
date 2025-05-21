using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimerManager : MonoBehaviour
{
    // Singleton estático
    public static CountdownTimerManager Instance { get; private set; }

    [Header("Timer Prefab (optional if this is the prefab itself)")]
    public GameObject timerPrefab;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        EnsureTimerExists();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureTimerExists();
        FindUIReferences();
        ResetTimer();
        if (SceneManager.GetActiveScene().name == "Creditos")
        {
            Destroy(gameObject);
        }
    }

    private void EnsureTimerExists()
    {
        if (Instance == null && timerPrefab != null)
        {
            GameObject newTimer = Instantiate(timerPrefab);
            newTimer.name = "CountdownTimer (Auto)";
        }
        else if (Instance != null && !Instance.gameObject.activeSelf)
        {
            Instance.gameObject.SetActive(true);
        }
    }

    private void FindUIReferences()
    {
        if (timerText == null)
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();

        if (timeAddedText == null)
            timeAddedText = GameObject.Find("TimeAddedText")?.GetComponent<TextMeshProUGUI>();
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
            timeAddedText.gameObject.SetActive(false);
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

            if (!isDisplayingTimeAdded)
            {
                timeAddedText.text = "¡Últimos 30 segundos!";
                timeAddedText.gameObject.SetActive(true);
                timeAddedDisplayTimer = 2f;
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
        if (timeAddedText != null)
        {
            timeAddedText.text = "¡Tiempo agotado!";
            timeAddedText.gameObject.SetActive(true);
            timeAddedDisplayTimer = 2f;
            isDisplayingTimeAdded = true;
        }

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
