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

    void Start()
    {
        // get vignette from volume
        postProcessVolume.profile.TryGet(out vignette);

        defaultColor = vignette.color.value;
        defaultIntensity = vignette.intensity.value;
    }

    void Update()
    {
        // smoothly return to normal
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultIntensity, Time.deltaTime * recoverSpeed);
        vignette.color.value = Color.Lerp(vignette.color.value, defaultColor, Time.deltaTime * recoverSpeed);
    }

    public void TakeDamageEffect()
    {
        // instantly flash stronger + red
        vignette.intensity.value = damageIntensity;
        vignette.color.value = damageColor;
    }
}