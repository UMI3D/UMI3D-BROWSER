using TMPro;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.UI;

public class ImageButtonDisplayer : SimpleButton
{
    private Image imageDisplayer;
    private TextMeshProUGUI textDisplayer;
    private RectTransform rectTransform;


    protected override void Awake()
    {
        base.Awake();
        imageDisplayer = GetComponent<Image>();
        textDisplayer = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitComponentData(Image sprite = null, float width = 0f, float height = 0f, float positionX = 0f, float positionY = 0f, string text = "null")
    {
        if (sprite != null)
        {
            imageDisplayer = sprite;
        }
        if (width != 0f || height != 0f)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }
        if (positionX != 0f || positionY != 0f)
        {
            rectTransform.anchoredPosition = new Vector2(positionX, positionY);
        }

        textDisplayer.text = text;

    }
}