using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform CameraTarget;

    private void Start()
    {
        CameraTarget = Camera.main.transform;
    }
    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        if (CameraTarget != null)
        {
            transform.LookAt(CameraTarget);

        }
        else
        {
            Debug.Log("No MainCamera found !");
        }
    }
}
