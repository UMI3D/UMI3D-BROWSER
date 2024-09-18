using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerOrientationPanel : MonoBehaviour
{
    public Transform playerCamera;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    public void OpenPanel()
    {
        this.transform.position = new Vector3(playerCamera.transform.position.x, 0.0f, playerCamera.transform.position.z);
        StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
    }

    public void ClosePanel()
    {
        StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
    }

    private System.Collections.IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
