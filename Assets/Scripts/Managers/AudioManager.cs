using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // Add references to your AudioSource components
    // public AudioSource musicSource;
    // public AudioSource sfxSource;

    public void PlayMusic(AudioClip clip)
    {
        // musicSource.clip = clip;
        // musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        // sfxSource.PlayOneShot(clip);
    }
}
