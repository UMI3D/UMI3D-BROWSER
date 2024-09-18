using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClientLBE
{
    public class ToggleCalibrationScene : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Button to manage orientation scene.")]
        private UnityEngine.UI.Toggle automaticCalibrationToggle;
       
        public GameObject orientationPanel;

        void Start()
        {
            Assert.IsNotNull(automaticCalibrationToggle);
            automaticCalibrationToggle.onValueChanged.AddListener(SwitchOrientationPanel);
        }

        public void SwitchOrientationPanel(bool value)
        {
            GuardianManager.Instance.ToggleCalibrationScene(value);
            orientationPanel.SetActive(value);
        }
    }
}
