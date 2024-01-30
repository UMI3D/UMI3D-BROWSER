using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SampleCreateUtility
{
    [MenuItem("GameObject/UMI3D_UI/Gameobject")]
    public static void CreateRandomGameobject(MenuCommand menuCommand)
    {
        CreateItemInHierarchyUtility.CreateObject("Sample object", typeof(AudioSource));
    }

    [MenuItem("GameObject/UMI3D_UI/Prefab")]
    public static void CreateRandomPrefab(MenuCommand menuCommand)
    {
        CreateItemInHierarchyUtility.CreatePrefab("Prefabs/XR Origin");
    }
}
