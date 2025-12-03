using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageVignette : MonoBehaviour
{
    public Volume postProcessVolume;
    private Vignette vignette;
    [SerializeField] private PlayerStats stats;
    private float lastrecordedhealth;

    [Header("Damage Vignette Settings")]
    public Color damageColor = Color.red;
    public float damageIntensity = 0.6f;
    public float recoverSpeed = 2f;

    private Color defaultColor;
    private float defaultIntensity;
    
    [Header("Additional Damage Sounds")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    
    
    //observer pattern
    private void OnEnable()
    {
        PlayerStats.OnHealthChanged += OnPlayerDamaged;
    }

    private void OnDisable()
    {
        PlayerStats.OnHealthChanged -= OnPlayerDamaged;
    }


    void Start()
    {
        postProcessVolume.profile.TryGet(out vignette);

        defaultColor = vignette.color.value;
        defaultIntensity = vignette.intensity.value;
        lastrecordedhealth = stats.getPlayerHealth();

    }

    void Update()
    {
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultIntensity, Time.deltaTime * recoverSpeed);
        vignette.color.value = Color.Lerp(vignette.color.value, defaultColor, Time.deltaTime * recoverSpeed);
    }
    
    private void OnPlayerDamaged(float newHealth)
    {
        TakeDamageEffect();
    }

    private void TakeDamageEffect()
    {
        vignette.intensity.value = damageIntensity;
        vignette.color.value = damageColor;
        AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.pitch = Random.Range(0.5f, 2f);
		audioSource.volume = Random.Range(0.5f, 100f);
        audioSource.PlayOneShot(audioClip);
    }
}