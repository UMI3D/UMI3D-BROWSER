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
        private UnityEngine.UI.Toggle AutomatiqueCalibrationToggle;
        private GuardianManager guardianManager;
       
        public GameObject OrientationPanel;

        void Start()
        {
            Assert.IsNotNull(AutomatiqueCalibrationToggle);
            AutomatiqueCalibrationToggle.onValueChanged.AddListener(SwitchOrientationPanel);
        }

        public void SwitchOrientationPanel(bool arg)
        {
            StartCoroutine(EnsureGuardianManagerAndToggle(arg));
        }

        private IEnumerator EnsureGuardianManagerAndToggle(bool arg)
        {
            while (guardianManager == null)
            {
                guardianManager = GameObject.FindObjectOfType<GuardianManager>();
                yield return new WaitForEndOfFrame();
            }

            guardianManager.ToggleCalibrationScene(arg);
            OrientationPanel.SetActive(arg);
        }
    }
}
