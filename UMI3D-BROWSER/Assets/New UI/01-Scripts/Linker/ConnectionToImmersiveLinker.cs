using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.connection;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/ConnectionToImmersive")]
    public class ConnectionToImmersiveLinker : ScriptableObject
    {
        private SetUpSkeleton setUpSkeleton;
        public SetUpSkeleton SetUpSkeleton => setUpSkeleton;
        public void SetSetUpSkeleton(SetUpSkeleton setUpSkeleton) { this.setUpSkeleton = setUpSkeleton; }

        public event Action OnDisplayEnvironmentHandler;
        public void DisplayEnvironmentHandler() { OnDisplayEnvironmentHandler?.Invoke(); }
        public event Action OnStopDisplayEnvironmentHandler;
        public void StopDisplayEnvironmentHandler() { OnStopDisplayEnvironmentHandler?.Invoke(); }

        public event Action<Transform> OnPlayerLoaded;
        public void PlayerLoaded(Transform playerTransform)
        { 
            OnPlayerLoaded?.Invoke(playerTransform);
        }

        public event Action OnLeave;
        public void Leave() { OnLeave?.Invoke(); }  
    }
}

