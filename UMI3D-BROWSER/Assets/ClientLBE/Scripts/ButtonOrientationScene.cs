using UnityEngine;
using UnityEngine.Assertions;

public class ButtonOrientationScene : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Button to manage orientation scene.")]
    private UnityEngine.UI.Button orientationSceneButton;

    private GetPlayerOrientationPanel panelOrientationScene;
    private bool onOffOrientationPanel = false;

    private void Start()
    {
        Assert.IsNotNull(orientationSceneButton);

        orientationSceneButton.onClick.AddListener(SwitchOrientationPanel);

        panelOrientationScene = GameObject.FindObjectOfType<GetPlayerOrientationPanel>();
    }

    [ContextMenu("Turn on/off Orientation panel choice before connection scene.")]
    void SwitchOrientationPanel()
    {

        if (panelOrientationScene == null)
        {
            panelOrientationScene = GameObject.FindObjectOfType<GetPlayerOrientationPanel>();
        }

        onOffOrientationPanel = !onOffOrientationPanel;

        if (onOffOrientationPanel)
        {
            panelOrientationScene.OpenPanel();
        }
        else
        {
            panelOrientationScene.ClosePanel();
        }
    }
}
