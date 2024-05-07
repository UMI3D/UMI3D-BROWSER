using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.linker;
using UnityEngine;

namespace umi3dBrowsers
{
    public class PlayerSpawner : MonoBehaviour
    {
        private ConnectionToImmersiveLinker m_linker;
        private Transform playerTransform;
        public void Init(ConnectionToImmersiveLinker linker)
        {
            Debug.Log("Prune");
            m_linker = linker;
            m_linker.OnPlayerLoaded += pt =>
            {
                Debug.Log("Hey");
                playerTransform = pt;
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

