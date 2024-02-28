using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportGestureDetection : MonoBehaviour
{
    [SerializeField] private GestureDetector _gestureDetector;

    private void OnEnable()
    {
        _gestureDetector.RightHand.NewActionDetected += OnActionDetetedRight;
        _gestureDetector.LeftHand.NewActionDetected += OnActionDetetedLeft;
    }

    private void OnDisable()
    {
        _gestureDetector.RightHand.NewActionDetected -= OnActionDetetedRight;
        _gestureDetector.LeftHand.NewActionDetected -= OnActionDetetedLeft;
    }

    private void OnActionDetetedRight(Gesture newGesture)
    {
        InputSystem.GetDevice<UMI3DXRInput>(CommonUsages.RightHand).Update(newGesture.name == "Teleport" || newGesture.name == "TeleportAction",
                                                     newGesture.name == "TeleportAction");
    }

    private void OnActionDetetedLeft(Gesture newGesture)
    {
        Debug.Log(newGesture.name);
        InputSystem.GetDevice<UMI3DXRInput>(CommonUsages.LeftHand).Update(newGesture.name == "Teleport" || newGesture.name == "TeleportAction",
                                                     newGesture.name == "TeleportAction");
    }
}
