using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3dBrowsers.ingame_ui;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.utils
{
    public class GridScroller : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private RectTransform gridRect;
        List<SpawnPointUI> spawnPoints;
        int lineAmount;
        int actualTopLine;
        float lineOffSet;

        private void Awake()
        {
            gridRect.anchoredPosition = Vector2.zero;
            spawnPoints = gridRect.GetComponentsInChildren<SpawnPointUI>().ToList();
            lineAmount = spawnPoints.Count / gridLayout.constraintCount;
            lineOffSet = gridLayout.cellSize.y + gridLayout.spacing.y;
        }

        public void GoUp()
        {
            if (spawnPoints == null) return;
            if (spawnPoints.Count > 0)
            {
                if (actualTopLine != 0)
                {
                    actualTopLine--;
                    Vector2 pos = gridRect.anchoredPosition;
                    pos.y = lineOffSet * actualTopLine;
                    gridRect.anchoredPosition = pos;
                }
            }
        }

        public void GoDown()
        {
            if (spawnPoints == null) return;
            if (spawnPoints.Count > 0)
            {
                if (actualTopLine < lineAmount - 1)
                {
                    actualTopLine++;
                    Vector2 pos = gridRect.anchoredPosition;
                    pos.y = lineOffSet * actualTopLine;
                    gridRect.anchoredPosition = pos;
                }
            }
        }
    }
}

