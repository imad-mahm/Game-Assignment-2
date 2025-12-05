using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
            AudioManager.Instance.PlaySfx(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null)
            AudioManager.Instance.PlaySfx(clickClip);
    }
}