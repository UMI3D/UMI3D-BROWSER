using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.ui.thumbnails;
using UnityEngine;

namespace umi3dBrowsers.container
{
    [CreateAssetMenu(menuName = "Data/Ui/VignetteContainer")]
    public class VignetteContainerData : ScriptableObject
    {
        [SerializeField] private ThumbnailContentMode m_vignetteScale;
        public ThumbnailContentMode VignetteScale => m_vignetteScale;
        [SerializeField] private Vector2 m_vignetteSize;
        public Vector2 VignetteSize => m_vignetteSize;
        [SerializeField] private Vector2 m_vignetteSpace;
        public Vector2 VignetteSpace => m_vignetteSpace;
        [SerializeField] private GameObject m_vignettePrefab;
        public GameObject VignettePrefab => m_vignettePrefab;
        [SerializeField] private int m_vignetteRowAmount;
        public int VignetteRowAmount => m_vignetteRowAmount;

        public static VignetteContainerData FindVignetteContainerDataByVignetteScale(ThumbnailContentMode vignetteScale, List<VignetteContainerData> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].m_vignetteScale == vignetteScale)
                    return datas[i];
            }

            return null;
        }
    }
}

