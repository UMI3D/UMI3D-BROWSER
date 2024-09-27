using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.sceneManagement;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linkers/MainContainerLinker")]
    public class MainContainerLinker : ScriptableObject
    {
        private Transform skyBox;
        public Transform Skybox => skyBox;

        public void SetSkyBoxTransform(Transform skyBox)
        {
            this.skyBox = skyBox;
        }

        private Light directionalLight;
        public Light DirectionalLight => directionalLight;
        public void SetDirectionalLight(Light directionalLight)
        {
            this.directionalLight = directionalLight;
        }

        private SceneLoader loader;
        public SceneLoader Loader => loader;

        public void SetLoader(SceneLoader loader)
        {
            this.loader = loader;
        }

        private PlayerSpawner spawner;
        public PlayerSpawner Spawner => spawner;
        public void SetSpawner(PlayerSpawner spawner)
        {
            this.spawner = spawner;
        }
    }
}

