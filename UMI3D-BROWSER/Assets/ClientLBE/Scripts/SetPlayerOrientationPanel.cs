using inetum.unityUtils;
using System.Collections;
using UnityEngine;

namespace ClientLBE
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SetPlayerOrientationPanel : SingleBehaviour<SetPlayerOrientationPanel>
    {
        public Camera playerCamera;
        public float fadeDuration = 0.5f;

        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public void OpenPanel()
        {
            this.transform.position = new Vector3(playerCamera.transform.position.x, 0.0f, playerCamera.transform.position.z);
            StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
        }

        public void ClosePanel()
        {
            StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
        }

        private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
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
}