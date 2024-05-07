using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.linker;
using UnityEngine;

namespace umi3dBrowsers
{
    public class PlayerSpy : MonoBehaviour
    {
        [SerializeField] private ConnectionToImmersiveLinker linker;
        private void Start()
        {
            Debug.Log("Cerise");
            linker.PlayerLoaded(transform);
        }
    }
}

