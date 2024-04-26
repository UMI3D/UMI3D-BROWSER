using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.services.title
{
    public enum TitleType { mainTitle, connectionTitle, subTitle}
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private MainMessageContainer maintTitleMessage;
        [SerializeField] private MainMessageContainer connectionTitleMessage;
        [SerializeField] private MainMessageContainer subTitleMessage;

        public void SetTitle(TitleType titleType,string prefix, string suffix, bool prefixOverride = false, bool suffixOverride = false)
        {
            maintTitleMessage.gameObject.SetActive(false);
            connectionTitleMessage.gameObject.SetActive(false);
            subTitleMessage.gameObject.SetActive(false);

            switch (titleType)
            {
                case TitleType.mainTitle:
                    maintTitleMessage.gameObject.SetActive(true);
                    maintTitleMessage.SetTitle(prefix, suffix, prefixOverride, suffixOverride); 
                    break;
                case TitleType.connectionTitle:
                    connectionTitleMessage.gameObject.SetActive(true);
                    connectionTitleMessage.SetTitle(prefix, suffix, prefixOverride, suffixOverride);
                    break;
                case TitleType.subTitle:
                    subTitleMessage.gameObject.SetActive(true);
                    subTitleMessage.SetPrefix(prefix, prefixOverride);
                    break;
            }
        }
    }
}

