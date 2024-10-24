using inetum.unityUtils.math;
using umi3d.browserRuntime.player;
using umi3dBrowsers.linker;
using UnityEngine;

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
            PlayerTransformUtils.RotatePlayerAndCenterCamera(playerTransform, Camera.main.transform, transform.rotation);
            PlayerTransformUtils.TranslatePlayerAndCenterCamera(playerTransform, Camera.main.transform, transform.position);
        }
    }
}

