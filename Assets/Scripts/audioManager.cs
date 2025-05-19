using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;  // Singleton instance
    public AudioSource soundEffectSource; // Drag an AudioSource here in the Inspector
    public AudioSource musicSource;

    public AudioClip ambientMusic;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Keep the AudioManager across scenes
    }

    private void Start()
    {
        PlayMusic(ambientMusic); 
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
}