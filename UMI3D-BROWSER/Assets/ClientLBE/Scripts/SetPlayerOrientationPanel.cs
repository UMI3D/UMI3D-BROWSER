using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientLBE
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SetPlayerOrientationPanel : SingleBehaviour<SetPlayerOrientationPanel>
    {
        public Camera playerCamera;
        public float fadeDuration = 0.5f;
        public List<GameObject> ButonOrientationPanel = new List<GameObject>();

        private CanvasGroup canvasGroup;

        public bool isPanelOpen = false;

        private void Start()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();

            for (int i = 0; i < ButonOrientationPanel.Count; i++)
            {
                ButonOrientationPanel[i].SetActive(false);
            }
        }

        public void OpenPanel()
        {
            for(int i = 0; i<ButonOrientationPanel.Count; i++)
            {
                ButonOrientationPanel[i].SetActive(true);
            }

            this.transform.position = new Vector3(playerCamera.transform.position.x, 0.0f, playerCamera.transform.position.z);
            StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
            isPanelOpen = true;
        }

        public void ClosePanel()
        {
            for (int i = 0; i < ButonOrientationPanel.Count; i++)
            {
                ButonOrientationPanel[i].SetActive(false);
            }
            StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
            isPanelOpen = false;

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