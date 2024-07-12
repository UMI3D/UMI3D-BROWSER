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
        public GuardianManager guardianManager;
        public GameObject OrientationPanel;

        void Start()
        {
            Assert.IsNotNull(AutomatiqueCalibrationToggle);

            AutomatiqueCalibrationToggle.onValueChanged.AddListener(SwitchOrientationPanel);
            guardianManager = GameObject.FindObjectOfType<GuardianManager>();
        }

        public void SwitchOrientationPanel(bool arg)
        {
            StartCoroutine(EnsureGuardianManagerAndToggle(arg));
        }

        private IEnumerator EnsureGuardianManagerAndToggle(bool arg)
        {
            if (guardianManager == null)
            {
                yield return new WaitForSeconds(0.1f);

                guardianManager = GameObject.FindObjectOfType<GuardianManager>();
                if (guardianManager == null)
                {
                    yield break;
                }
            }

            guardianManager.ToggleCalibrationScene(arg);
            OrientationPanel.SetActive(arg);
        }
    }
}
