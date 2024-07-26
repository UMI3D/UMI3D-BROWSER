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
            playerTransform.position = transform.position;
            playerTransform.rotation = transform.rotation;

            var cameraTransform = Camera.main.transform;
            cameraTransform.parent.localEulerAngles = new Vector3(0, -cameraTransform.localEulerAngles.y, 0);
            cameraTransform.parent.localPosition = new Vector3(-cameraTransform.localPosition.x, 0, -cameraTransform.localPosition.y);
        }
    }
}

