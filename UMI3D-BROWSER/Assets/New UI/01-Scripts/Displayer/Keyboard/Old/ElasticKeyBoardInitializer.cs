using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.UX;
using umi3dBrowsers.linker;
using umi3dBrowsers.utils;
using UnityEngine;

namespace umi3dBrowsers.keyboard
{
    public class ElasticKeyBoardInitializer : MonoBehaviour
    {
        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;
        LazyRotationAndTranslation lazyRotationAndTranslation;

        private void Awake()
        {
            connectionToImmersiveLinker.OnPlayerLoaded += playerTransform => StartCoroutine(SetParent());
            lazyRotationAndTranslation = transform.parent.GetComponent<LazyRotationAndTranslation>();
        }

        private void Start()
        {
            //StartCoroutine(SetParent());
        }

        IEnumerator SetParent()
        {
            while (Camera.main == null) 
            {
                yield return null;
            }

            lazyRotationAndTranslation.target = Camera.main.transform;
        }
    }
}
