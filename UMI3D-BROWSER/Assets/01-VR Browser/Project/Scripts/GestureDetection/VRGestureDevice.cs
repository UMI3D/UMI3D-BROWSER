using System.Collections.Generic;
using System.Linq;

using umi3d.picoBrowser;

using umi3dVRBrowsersBase.interactions;

using UnityEngine;

public class VRGestureDevice : MonoBehaviour
{
    [SerializeField]
    private ControllerType controllerType;
    public ControllerType ControllerType => controllerType;

    [SerializeField]
    private List<VRGestureObserver> gestureInputs = new();
    public IReadOnlyList<VRGestureObserver> GestureInputs => gestureInputs;

    [SerializeField]
    private List<VRPokeInputObserver> pokeInputs = new();
    public IReadOnlyList<VRPokeInputObserver> PokeInputs => pokeInputs;


    private void Start()
    {
        foreach (VRGestureObserver observer in GetComponentsInChildren<VRGestureObserver>())
        {
            if (gestureInputs!.Contains(observer))
                gestureInputs.Add(observer);
        }
        gestureInputs = gestureInputs.Where(x => x != null).ToList();
        pokeInputs = pokeInputs.Where(x => x != null).ToList();
    }
}
