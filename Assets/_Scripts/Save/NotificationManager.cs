using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayTime = 3f;

    private void Awake()
    {
        Instance = this;
        notificationText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message, Color color)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageRoutine(message, color));
    }

    private IEnumerator ShowMessageRoutine(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;

        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        float fadeTime = 1f;
        float t = 0;

        Color startColor = notificationText.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            notificationText.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (t / fadeTime));
            yield return null;
        }

        notificationText.gameObject.SetActive(false);

        notificationText.color = new Color(startColor.r, startColor.g, startColor.b, 1);
    }
}