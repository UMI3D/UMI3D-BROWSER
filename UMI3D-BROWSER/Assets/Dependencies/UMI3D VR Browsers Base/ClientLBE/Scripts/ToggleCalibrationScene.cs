using UnityEngine;
using UnityEngine.UI;

namespace umi3d.VRBase.lbe
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleCalibrationScene : MonoBehaviour
    {
        public ButtonOrientationScene orientationPanel;
        private Toggle automaticCalibrationToggle;

        void Start()
        {
            automaticCalibrationToggle = this.GetComponent<Toggle>();
            automaticCalibrationToggle.onValueChanged.AddListener(ToggleManualCalibrator);
            automaticCalibrationToggle.isOn = true;
        }

        public void ToggleManualCalibrator(bool value)
        {
            //GuardianManager.Instance.ToggleCalibrationScene(value);       
        }
    }
}