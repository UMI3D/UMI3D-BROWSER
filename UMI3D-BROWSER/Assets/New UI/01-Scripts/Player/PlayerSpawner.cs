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
        private Transform parentTransform;
        [SerializeField] private GameObject playerPrefab;

        public void Init(ConnectionToImmersiveLinker linker)
        {
            m_linker = linker;
            m_linker.OnPlayerLoaded += (pT) =>
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

        public void SpawnPlayer()
        {
            Instantiate(playerPrefab, transform.position, transform.rotation);
        }
    }
}

