using UnityEngine;
using UnityEngine.UI;

namespace ClientLBE
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleCalibrationScene : MonoBehaviour
    {
        public ButtonOrientationScene orientationPanel;
        private Toggle automaticCalibrationToggle;

        void Start()
        {
            automaticCalibrationToggle = this.GetComponent<Toggle>();
            automaticCalibrationToggle.onValueChanged.AddListener(SwitchOrientationPanel);
        }

        public void SwitchOrientationPanel(bool value)
        {
            GuardianManager.Instance.ToggleCalibrationScene(value);
            orientationPanel.gameObject.SetActive(value);
        }
    }
}