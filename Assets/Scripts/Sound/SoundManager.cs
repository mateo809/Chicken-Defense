using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Assignables")]
    [SerializeField]
    private Slider masterSoundVolume;
    [SerializeField]
    private Slider musicSoundVolume;
    [SerializeField]
    private Slider sfxSoundVolume;
    [Space(10)]
    [Header("Settings Volumes")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public UnityEvent musicChangedEvent;
    public UnityEvent sfxChangedEvent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadSoundVolume();
    }

    private void LoadSoundVolume()
    {
        masterVolume = PlayerPrefs.GetFloat("masterSoundVolume", 1);
        musicVolume = PlayerPrefs.GetFloat("musicSoundVolume", 1);
        sfxVolume = PlayerPrefs.GetFloat("sfxSoundVolume", 1);

        masterSoundVolume.value = masterVolume;
        musicSoundVolume.value = musicVolume;
        sfxSoundVolume.value = sfxVolume;
    }

    public void OnMasterChanged()
    {
        masterVolume = masterSoundVolume.value;
        PlayerPrefs.SetFloat("masterSoundVolume", masterVolume);
        musicChangedEvent.Invoke();
        sfxChangedEvent.Invoke();
    }

    public void OnMusicChanged()
    {
        musicVolume = musicSoundVolume.value;
        PlayerPrefs.SetFloat("musicSoundVolume", musicVolume);
        musicChangedEvent.Invoke();
    }

    public void OnSFXChanged()
    {
        sfxVolume = sfxSoundVolume.value;
        PlayerPrefs.SetFloat("sfxSoundVolume", sfxVolume);
        sfxChangedEvent.Invoke();
    }
}
