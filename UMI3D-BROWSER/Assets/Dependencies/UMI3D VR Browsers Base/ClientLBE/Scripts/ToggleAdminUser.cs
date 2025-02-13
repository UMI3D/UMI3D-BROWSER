using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.VRBase.lbe
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleAdminUser : MonoBehaviour
    {
        public GameObject CheckToggleUserAdmin;
        private Toggle SetUserAdmin;

        void Start()
        {
            SetUserAdmin = this.GetComponent<Toggle>();
            SetUserAdmin.onValueChanged.AddListener(SwitchOrientationPanel);
        }

        public void SwitchOrientationPanel(bool value)
        {
            //GuardianManager.Instance.ToggleUserAdmin(value);
            CheckToggleUserAdmin.SetActive(value);
        }
    }
}
