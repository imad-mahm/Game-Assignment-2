using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleAmount = 1.1f;
    [SerializeField] private float speed = 8f;

    private Vector3 originalScale;
    private bool hovering = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 targetScale = hovering ? originalScale * scaleAmount : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}