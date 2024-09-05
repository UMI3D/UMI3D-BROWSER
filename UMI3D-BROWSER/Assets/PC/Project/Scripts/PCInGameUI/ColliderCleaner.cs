using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3dBrowsers.utils;
using UnityEngine;

namespace umi3dBrowsers.pc.ingame_ui
{
    public class ColliderCleaner : MonoBehaviour
    {
        private void Awake()
        {
            GetComponentsInChildren<Collider>().ToList().ForEach(c =>
            {
                c.enabled = false;
            });

            GetComponentsInChildren<UIColliderScaller>().ToList().ForEach(c =>
            {
                c.enabled = false;
            });
        }
    }
}
