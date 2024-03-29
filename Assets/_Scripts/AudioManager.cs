using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSound, sfxSound;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name, float volume)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            Debug.Log("SFX played");
            sfxSource.clip = s.clip;
            sfxSource.volume = volume;
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void PauseSFX(string name)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            Debug.Log("SFX played");
            sfxSource.clip = s.clip;
            sfxSource.Stop();
        }
    }

    public void PauseAllSound()
    {
        sfxSource.Pause();
        musicSource.Pause();
    }

    public void UnPauseAllSound()
    {
        sfxSource.UnPause();
        musicSource.UnPause();
    }
}
