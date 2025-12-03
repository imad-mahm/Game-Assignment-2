using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageVignette : MonoBehaviour
{
    public Volume postProcessVolume;
    private Vignette vignette;

    [Header("Damage Vignette Settings")]
    public Color damageColor = Color.red;
    public float damageIntensity = 0.6f;
    public float recoverSpeed = 2f;

    private Color defaultColor;
    private float defaultIntensity;
    
    [Header("Additional Damage Sounds")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    void Start()
    {
        // get vignette from volume
        postProcessVolume.profile.TryGet(out vignette);

        defaultColor = vignette.color.value;
        defaultIntensity = vignette.intensity.value;
        
        //TODO: Get the sounds of damage we're gonna make and add them here
    }

    void Update()
    {
        // smoothly return to normal
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultIntensity, Time.deltaTime * recoverSpeed);
        vignette.color.value = Color.Lerp(vignette.color.value, defaultColor, Time.deltaTime * recoverSpeed);

        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamageEffect();
        }
    }

    public void TakeDamageEffect()
    {
        // instantly flash stronger + red
        vignette.intensity.value = damageIntensity;
        vignette.color.value = damageColor;
        // play a random sound from clips at random pitches and volumes
        AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.pitch = Random.Range(0.5f, 2f);
		audioSource.volume = Random.Range(0.5f, 100f);
        audioSource.PlayOneShot(audioClip);
    }
}