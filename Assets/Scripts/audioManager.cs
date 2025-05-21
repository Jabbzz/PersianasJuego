using UnityEngine;
using UnityEngine.SceneManagement;


public class audioManager : MonoBehaviour
{
    public static audioManager instance;  // Singleton instance
    public AudioSource soundEffectSource; // Drag an AudioSource here in the Inspector
    public AudioSource musicSource;

    public AudioClip ambientMusicNivel1;
    public AudioClip ambientMusicNivel2;
    public AudioClip ambientMusicNivel3;
    public AudioClip ambientMusicNivel4;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Keep the AudioManager across scenes
    }

    public void PlaySound(AudioClip clip)
    {
        soundEffectSource.PlayOneShot(clip); // Play a one-time sound
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource.clip != musicClip)
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void OnDestroy()
    {
        // Desuscribir para evitar errores
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cambiar música según nombre de la escena
        switch (scene.name)
        {
            case "MainMenu":
                musicSource.volume = 0.1f;
                PlayMusic(ambientMusicNivel1);
                break;
            case "Nivel1":
                musicSource.volume = 0.1f;
                PlayMusic(ambientMusicNivel2);
                break;
            case "Nivel2":
                musicSource.volume = 0.1f;
                PlayMusic(ambientMusicNivel3);
                break;
            case "Nivel3":
            musicSource.volume = 0.1f;
            PlayMusic(ambientMusicNivel4);
            break;
            default:
                musicSource.Stop();  // O reproducir música por defecto
                break;
        }
    }
}