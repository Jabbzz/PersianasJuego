using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;  // Singleton instance
    public AudioSource soundEffectSource; // Drag an AudioSource here in the Inspector

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

    public void PlaySound(AudioClip clip)
    {
        soundEffectSource.PlayOneShot(clip); // Play a one-time sound
    }
}