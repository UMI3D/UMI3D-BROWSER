using UnityEngine;

namespace umi3dBrowsers.data.ui
{
    [CreateAssetMenu(menuName = "Data/Ui/Popup")]
    public class PopupData : ScriptableObject
    {
        [SerializeField] private GameObject m_prefab;

        public GameObject Prefab => m_prefab;
    }
}