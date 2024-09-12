using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3dBrowsers.linker;
using UnityEngine;

namespace umi3dBrowsers.ingame_ui.spawnPoints
{
    public class SpawnPointsUIManager : MonoBehaviour
    {
        [SerializeField] private RectTransform spawnPointsParent;
        [SerializeField] private GameObject SpawnPointPrefab;
        [SerializeField] private SpawnPointLinker linker;

        List<SpawnPointUI> spawnPoints;

        private void Awake()
        {
            spawnPoints = new();
            spawnPoints.AddRange(spawnPointsParent.GetComponentsInChildren<SpawnPointUI>());
            linker.OnSpawnPointReceived += points => AddSpawnPoints(points);
        }

        public void AddSpawnPoints(List<string> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                spawnPoints.Add(Instantiate(SpawnPointPrefab, spawnPointsParent).GetComponent<SpawnPointUI>());
                spawnPoints.Last().Init(points[i], i);
            }
        }
    }

}
