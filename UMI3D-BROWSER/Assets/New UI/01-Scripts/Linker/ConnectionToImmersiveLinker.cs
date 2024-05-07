using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.connection;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/ConnectionToImmersive")]
    public class ConnectionToImmersiveLinker : MonoBehaviour
    {
        private SetUpSkeleton setUpSkeleton;
        public SetUpSkeleton SetUpSkeleton => setUpSkeleton;
        public void SetSetUpSkeleton(SetUpSkeleton setUpSkeleton)
        {
            this.setUpSkeleton = setUpSkeleton;
        }
    }
}

