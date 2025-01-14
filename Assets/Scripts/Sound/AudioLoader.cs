using UnityEngine;

public class AudioLoader : MonoBehaviour
{
    public AudioType type;
    private AudioSource source;
    private SoundManager soundManager;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        soundManager = SoundManager.instance;

        if (soundManager != null)
        {
            switch (type)
            {
                case AudioType.Music:
                    soundManager.musicChangedEvent.AddListener(RefreshMusicVolume);
                    break;
                case AudioType.Effect:
                    soundManager.sfxChangedEvent.AddListener(RefreshSFXVolume);
                    break;
                default:
                    break;
            }
        }

        RefreshVolume();
    }

    private void RefreshVolume()
    {
        if (soundManager != null)
        {
            switch (type)
            {
                case AudioType.Music:
                    source.volume = soundManager.masterVolume * soundManager.musicVolume;
                    break;
                case AudioType.Effect:
                    source.volume = soundManager.masterVolume * soundManager.sfxVolume;
                    break;
                default:
                    break;
            }
        }
    }

    public void RefreshMusicVolume()
    {
        source.volume = soundManager.masterVolume * soundManager.musicVolume;
    }

    public void RefreshSFXVolume()
    {
        source.volume = soundManager.masterVolume * soundManager.sfxVolume;
    }

    private void OnDestroy()
    {
        if (soundManager != null)
        {
            switch (type)
            {
                case AudioType.Music:
                    soundManager.musicChangedEvent.RemoveListener(RefreshMusicVolume);
                    break;
                case AudioType.Effect:
                    soundManager.sfxChangedEvent.RemoveListener(RefreshSFXVolume);
                    break;
                default:
                    break;
            }
        }
    }
}

public enum AudioType
{
    Music,
    Effect
}