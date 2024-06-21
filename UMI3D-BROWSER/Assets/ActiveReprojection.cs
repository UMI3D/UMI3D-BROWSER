using Meta.XR;
using Pico.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveReprojection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MetaXRSpaceWarp.SetSpaceWarp(true);
        OVRManager.SetSpaceWarp(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
