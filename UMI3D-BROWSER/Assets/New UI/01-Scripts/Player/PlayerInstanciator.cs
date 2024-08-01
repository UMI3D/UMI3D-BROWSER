using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using umi3d.browserRuntime.conditionalCompilation;
using UnityEngine;

namespace umi3dBrowsers.player
{
    public class PlayerInstanciator : MonoBehaviour
    {
        [SerializeField] private MultiDeviceReference<GameObject> playerPrefab;

        private void Awake()
        {
            Instantiate(playerPrefab.Reference);
        }
    }
}

