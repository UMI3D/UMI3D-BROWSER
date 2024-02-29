using System.Collections.Generic;
using System.Linq;

using umi3d.cdk.interaction;

using umi3dVRBrowsersBase.interactions.selection;
using umi3dVRBrowsersBase.interactions.selection.selector;
using umi3dVRBrowsersBase.ui;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Samples.GestureSample;

/// <summary>
/// Disable ray when specific gestures are performed.
/// </summary>
public class HandTrackingRayManager : MonoBehaviour
{
    [SerializeField, Tooltip("When at least one of those gestures is performed, ray selection is disabled.")]
    private List<StaticHandGesture> gesturesDisablingRay = new();

    private Dictionary<StaticHandGesture, bool> gesturePerforming = new();

    private bool isRayDisabled = false;

    [SerializeField]
    private VRSelectionManager selectionManager;

    void Start()
    {
        selectionManager = selectionManager == null ? GetComponent<VRSelectionManager>() : selectionManager;

        foreach (StaticHandGesture gestureListener in gesturesDisablingRay)
        {
            gesturePerforming.Add(gestureListener, false);
            gestureListener.GesturePerformed.AddListener(() =>
            {
                gesturePerforming[gestureListener] = true;
                TryDisableRay(); 
            });
            gestureListener.GestureEnded.AddListener(() =>
            {
                gesturePerforming[gestureListener] = false;
                EnableRay();
            });
        }
    }

    private void TryDisableRay()
    {
        if (!isRayDisabled && gesturePerforming.Values.Any(x => x))
            DisableRay();
    }

    private void DisableRay()
    {
        selectionManager.interactableSelector.SelectionParadigmMode = AbstractVRSelector<InteractableContainer>.SelectionParadigm.PROXIMITY_ONLY;
        selectionManager.selectableSelector.SelectionParadigmMode = AbstractVRSelector<Selectable>.SelectionParadigm.PROXIMITY_ONLY;
        selectionManager.elementSelector.SelectionParadigmMode = AbstractVRSelector<AbstractClientInteractableElement>.SelectionParadigm.PROXIMITY_ONLY;
        isRayDisabled = true;
        selectionManager.pointingCursor.Hide();

    }

    private void EnableRay()
    {
        selectionManager.interactableSelector.SelectionParadigmMode = AbstractVRSelector<InteractableContainer>.SelectionParadigm.ALL;
        selectionManager.selectableSelector.SelectionParadigmMode = AbstractVRSelector<Selectable>.SelectionParadigm.ALL;
        selectionManager.elementSelector.SelectionParadigmMode = AbstractVRSelector<AbstractClientInteractableElement>.SelectionParadigm.ALL;
        isRayDisabled = false;
        selectionManager.pointingCursor.Display();
    }
}
