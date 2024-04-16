using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.container
{
    [Tooltip("Should be on every Content panel")]
    public class ContentContainer : MonoBehaviour
    {
        [SerializeField] private string prefixTitleKey;
        [SerializeField] private string suffixTitleKey;

        public string PrefixTitleKey => prefixTitleKey;
        public string SuffixTitleKey => suffixTitleKey;
    }
}
