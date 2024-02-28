using System.Collections;
using System.Collections.Generic;

using umi3d.picoBrowser;

using umi3dVRBrowsersBase.interactions;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[RequireComponent(typeof(XRInputModalityManager))]
public class XRInputModalityListener : MonoBehaviour
{
    private XRInputModalityManager XRInputModalityManager;

    void Start()
    {
        XRInputModalityManager = GetComponent<XRInputModalityManager>();

        XRInputModalityManager.trackedHandModeStarted.AddListener(AddHand);
        XRInputModalityManager.motionControllerModeStarted.AddListener(AddPhysicalDevice);
    }

    void AddPhysicalDevice()
    {

        List<InputDevice> queryResult = new(); //maybe remove held in hand

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, queryResult);
        InputDevice leftController = queryResult.Count > 0 ? queryResult[0] : default;
        queryResult.Clear();

        if (leftController != default)
            ((Umi3dVRInputManager)Umi3dVRInputManager.Instance).AddPhysicalDevice(ControllerType.LeftHandController, leftController);

        InputDevices.GetDeviceAtXRNode(XRNode.RightEye);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        queryResult.Clear();

        if (leftController != default)
            ((Umi3dVRInputManager)Umi3dVRInputManager.Instance).AddPhysicalDevice(ControllerType.RightHandController, rightController);
    }

    void AddHand()
    {
        List<InputDevice> queryResult = new();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.HandTracking, queryResult);
        InputDevice leftController = queryResult.Count > 0 ? queryResult[0] : default;
        queryResult.Clear();

        if (leftController != default)
            ((Umi3dVRInputManager)Umi3dVRInputManager.Instance).AddHandTrackedDevice(ControllerType.LeftHandController, leftController);

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.HandTracking, queryResult);
        InputDevice rightController = queryResult.Count > 0 ? queryResult[0] : default;
        queryResult.Clear();

        if (rightController != default)
            ((Umi3dVRInputManager)Umi3dVRInputManager.Instance).AddHandTrackedDevice(ControllerType.RightHandController, rightController);

        foreach (VRGestureDevice gestureDevice in UnityEngine.Object.FindObjectsOfType<VRGestureDevice>(true))
            ((Umi3dVRInputManager)Umi3dVRInputManager.Instance).AddHandTrackedGestureDevice(gestureDevice);
    }
}
