using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ImageDisplayer : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}

