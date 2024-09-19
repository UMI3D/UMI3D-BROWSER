using UnityEngine;
using UnityEngine.UI;

namespace ClientLBE
{
    [RequireComponent(typeof(Button))]
    public class ButtonOrientationScene : MonoBehaviour
    {
        private Button orientationSceneButton;
        private bool onOffOrientationPanel = false;

        private void Start()
        {
            orientationSceneButton = this.GetComponent<Button>();
            orientationSceneButton.onClick.AddListener(SwitchOrientationPanel);
        }

        [ContextMenu("Turn on/off Orientation panel choice before connection scene.")]
        void SwitchOrientationPanel()
        {
            onOffOrientationPanel = !onOffOrientationPanel;

            if (onOffOrientationPanel)
                SetPlayerOrientationPanel.Instance.OpenPanel();

            else
                SetPlayerOrientationPanel.Instance.ClosePanel();
        }
    }
}
