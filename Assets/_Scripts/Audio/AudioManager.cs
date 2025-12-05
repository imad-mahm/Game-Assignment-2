using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;    // Main music channel
    [SerializeField] private AudioSource sfxSource;      // SFX channel

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Default Volumes")]
    [Range(0f, 1f)] public float defaultMusicVolume = 0.3f;
    [Range(0f, 1f)] public float defaultSfxVolume = 1f;

    private const string MusicVolKey = "MusicVolume";
    private const string SfxVolKey   = "SfxVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        float musicVol = PlayerPrefs.GetFloat(MusicVolKey, defaultMusicVolume);
        float sfxVol   = PlayerPrefs.GetFloat(SfxVolKey, defaultSfxVolume);

        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);
    }

    private void Start()
    {
        if (menuMusic != null)
            PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {   
        PlayMusic(menuMusic, true);
        Debug.Log("Menu music");
    }
    public void PlayGameplayMusic()
    {
        if (gameplayMusic == null)
        {
            musicSource.Stop();
            return;
        }

        PlayMusic(gameplayMusic, true);
        Debug.Log("Gameplay music");
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MusicVolKey, volume);
    }

    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SfxVolKey, volume);
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
