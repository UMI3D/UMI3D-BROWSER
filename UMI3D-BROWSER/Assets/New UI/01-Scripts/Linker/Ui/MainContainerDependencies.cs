using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.sceneManagement;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    public class MainContainerDependencies : MonoBehaviour
    {
        [SerializeField] private MainContainerLinker mainContainerLinker;
        [SerializeField] private Transform skyBox;
        [SerializeField] private Light directionalLight;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private PlayerSpawner spawner;

        private void Awake()
        {
            mainContainerLinker.SetSkyBoxTransform(skyBox);
            mainContainerLinker.SetDirectionalLight(directionalLight);
            mainContainerLinker.SetLoader(sceneLoader);
            mainContainerLinker.SetSpawner(spawner);
        }
    }

}
