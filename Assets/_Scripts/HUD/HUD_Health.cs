using UnityEngine;
using UnityEngine.UI;

public class HUD_Health : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float smoothSpeed = 5f;

    private float currentFill;   
    private float targetFill;    

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponentInChildren<Image>();

        currentFill = targetFill = fillImage.fillAmount;
    }

    private void Update()
    {
        currentFill = Mathf.Lerp(currentFill, targetFill, smoothSpeed * Time.deltaTime);
        fillImage.fillAmount = currentFill;
    }

    public void UpdateHealth(float normalizedValue)
    {
        targetFill = Mathf.Clamp01(normalizedValue);
    }
}