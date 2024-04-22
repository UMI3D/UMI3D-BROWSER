using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.container
{
    public class VignetteContainerEvent : MonoBehaviour
    {
        [SerializeField] private List<VignetteContainer> lstVignetteContainers;

        private void Awake()
        {
            foreach (var container in lstVignetteContainers)
            {
                foreach (var container2 in lstVignetteContainers)
                {
                    container.OnReset += container2.ResetVignettes;
                }
            }
        }
    }
}