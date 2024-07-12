using UnityEngine;
using UnityEngine.Assertions;

public class ButtonOrientationScene : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Button to manage orientation scene.")]
    private UnityEngine.UI.Button OrientationSceneButton;

    private GetPlayerOrientationPanel PanelOrientationScene;
    private bool OnOffOrientationPanel = false;

    private void Start()
    {
        Assert.IsNotNull(OrientationSceneButton);

        OrientationSceneButton.onClick.AddListener(SwitchOrientationPanel);

        PanelOrientationScene = GameObject.FindObjectOfType<GetPlayerOrientationPanel>();
    }

    [ContextMenu("Turn on/off Orientation panel choice before connection scene.")]
    void SwitchOrientationPanel()
    {

        if (PanelOrientationScene == null)
        {
            PanelOrientationScene = GameObject.FindObjectOfType<GetPlayerOrientationPanel>();
        }

        OnOffOrientationPanel = !OnOffOrientationPanel;

        if (OnOffOrientationPanel)
        {
            PanelOrientationScene.OpenPanel();
        }
        else
        {
            PanelOrientationScene.ClosePanel();
        }
    }
}
