using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ImageDisplayer : MonoBehaviour, IDisplayer
    {
        [SerializeField] private Image image;

        public object GetValue(bool trim)
        {
            throw new System.NotImplementedException();
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            throw new System.NotImplementedException();
        }

        public void SetResource(object resource)
        {
            image.sprite = (Sprite)resource;
        }

        public void SetTitle(string title)
        {
            throw new System.NotImplementedException();
        }
    }
}

