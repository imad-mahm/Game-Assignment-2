using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Optional Test SFX")]
    [SerializeField] private AudioClip testSfx;

    private bool isInitialized = false;

    private void OnEnable()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float savedSfx   = PlayerPrefs.GetFloat("SfxVolume", 1f);

        if (musicSlider != null)
            musicSlider.value = savedMusic;

        if (sfxSlider != null)
            sfxSlider.value = savedSfx;

        if (!isInitialized)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            isInitialized = true;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);

        if (testSfx != null)
            AudioManager.Instance.PlaySfx(testSfx);
    }

    public void ResetToDefaults()
    {
        float defaultMusic = 0.3f;
        float defaultSfx   = 1f;

        musicSlider.value = defaultMusic;
        sfxSlider.value = defaultSfx;

        AudioManager.Instance.SetMusicVolume(defaultMusic);
        AudioManager.Instance.SetSfxVolume(defaultSfx);

        PlayerPrefs.SetFloat("MusicVolume", defaultMusic);
        PlayerPrefs.SetFloat("SfxVolume", defaultSfx);
        PlayerPrefs.Save();
    }
}