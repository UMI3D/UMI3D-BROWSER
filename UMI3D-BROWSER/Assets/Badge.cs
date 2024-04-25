using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Badge : MonoBehaviour
{
    public TextMeshProUGUI BadgeText;
    [SerializeField] RectTransform textTransform;
    [SerializeField] RectTransform backgroundTransform;
    [SerializeField] float padding = 10f;

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
    }

    private void OnGUI()
    {
        backgroundTransform.sizeDelta = new Vector2(textTransform.sizeDelta.x + 2f * padding, backgroundTransform.sizeDelta.y);
    }
}