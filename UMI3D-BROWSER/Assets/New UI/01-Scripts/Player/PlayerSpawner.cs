using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.linker;
using UnityEngine;
using static umi3dBrowsers.MainContainer;

namespace umi3dBrowsers
{
    public class PlayerSpawner : MonoBehaviour
    {
        private Transform playerTransform;

        [Header("Linkers")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;

        public void Awake()
        {
            connectionToImmersiveLinker.OnPlayerLoaded += (pT) =>
            {
                playerTransform = pT;
                RepositionPlayer();
            };
        }
        public void RepositionPlayer()
        {
            playerTransform.position = transform.position;
            playerTransform.rotation = transform.rotation;  
        }
    }
}

