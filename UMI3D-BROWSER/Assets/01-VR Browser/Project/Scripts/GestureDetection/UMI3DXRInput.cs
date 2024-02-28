using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR;

public struct CustomXrState : IInputStateTypeInfo
{
    public FourCC format => new FourCC("UMIX");

    [Preserve, InputControl(name = "Teleport", layout = "vector2")]
    public Vector2 Teleport;
    [Preserve, InputControl(name = "TeleportSelect", layout = "vector2")]
    public Vector2 TeleportSelect;
}

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
[Preserve, InputControlLayout(displayName = "UMI3D Hand", stateType = typeof(CustomXrState), commonUsages = new[] { "LeftHand", "RightHand" })]
public class UMI3DXRInput : UnityEngine.InputSystem.InputDevice
{
    const string k_UMI3DAimHandDeviceProductName = "UMI3D Aim Hand Tracking";
    const string k_UMI3DAimHandDeviceManufacturerName = "OpenXR UMI3D";

    public static UMI3DXRInput Right;
    public static UMI3DXRInput Left;

    public Vector2Control Teleport { get; private set; }
    public Vector2Control TeleportSelect { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        Teleport = GetChildControl<Vector2Control>(nameof(Teleport));
        TeleportSelect = GetChildControl<Vector2Control>(nameof(TeleportSelect));

        var deviceDescriptor = XRDeviceDescriptor.FromJson(description.capabilities);
        if (deviceDescriptor != null)
        {
            if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Left) != 0)
                InputSystem.SetDeviceUsage(this, UnityEngine.InputSystem.CommonUsages.LeftHand);
            else if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Right) != 0)
                InputSystem.SetDeviceUsage(this, UnityEngine.InputSystem.CommonUsages.RightHand);
        }
    }
    public static UMI3DXRInput CreateHand(InputDeviceCharacteristics characteristics)
    {
        var desc = new InputDeviceDescription
        {
            product = k_UMI3DAimHandDeviceProductName,
            manufacturer = k_UMI3DAimHandDeviceManufacturerName,
            capabilities = new XRDeviceDescriptor 
            {
                characteristics = characteristics,
                inputFeatures = new List<XRFeatureDescriptor>
                    {
                        new XRFeatureDescriptor
                        {
                            name = "Teleport",
                            featureType = FeatureType.Axis2D
                        },
                        new XRFeatureDescriptor
                        {
                            name = "TeleportAction",
                            featureType = FeatureType.Axis2D
                        }
                    }
            }.ToJson()
        };
        return InputSystem.AddDevice(desc) as UMI3DXRInput;
    }


#if UNITY_EDITOR
    static UMI3DXRInput() => RegisterLayout();
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterLayout()
    {
        InputSystem.RegisterLayout<UMI3DXRInput>(
                matches: new InputDeviceMatcher()
                .WithProduct(k_UMI3DAimHandDeviceProductName)
                .WithManufacturer(k_UMI3DAimHandDeviceManufacturerName));

        Right = CreateHand(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller);
        Left = CreateHand(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller);
    }

    public void Update(bool teleportePose, bool teleporteAction)
    {
        InputSystem.QueueDeltaStateEvent(Teleport, new Vector2(0, teleportePose ? 1 : 0));
        InputSystem.QueueDeltaStateEvent(TeleportSelect, new Vector2(0, teleporteAction ? 1 : 0));
    }

    [MenuItem("Tools/Add Device")]
    public static void Init()
    {
        InputSystem.AddDevice<UMI3DXRInput>();
    }
}
