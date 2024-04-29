using UnityEngine;
using UnityEngine.UI;
using TMPro;
using umi3dBrowsers.utils;

public class Badge : MonoBehaviour
{
    public TextMeshProUGUI BadgeText;
    [SerializeField] RectTransform textTransform;
    [SerializeField] RectTransform backgroundTransform;
    [SerializeField] float padding = 10f;

    private UIColliderScaller scaller;

    private void Awake()
    {
        if (BadgeText == null)
        {
            BadgeText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (textTransform == null)
        {
            textTransform = BadgeText.transform as RectTransform;
        }
        if (backgroundTransform == null)
        {
            backgroundTransform = GetComponentInChildren<Image>().transform as RectTransform;
        }

        scaller = GetComponent<UIColliderScaller>();
    }

    private void OnGUI()
    {
        backgroundTransform.sizeDelta = new Vector2(textTransform.sizeDelta.x + 2f * padding, textTransform.sizeDelta.y + padding);
        scaller?.ScaleCollider();
    }
}