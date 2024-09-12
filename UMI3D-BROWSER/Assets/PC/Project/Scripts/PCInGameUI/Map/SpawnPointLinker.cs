using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linkers/SpawnPoint")]
    public class SpawnPointLinker : ScriptableObject
    {

        public event Action<List<string>> OnSpawnPointReceived;
        public void SpawnPointsReceived(List<string> points)
        {
            OnSpawnPointReceived?.Invoke(points);
        }
    }
}

