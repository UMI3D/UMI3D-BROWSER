using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildChecker : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(UMI3DCollabLoadingParameters).Name);

        bool found = false;


        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var loadingParam =  AssetDatabase.LoadAssetAtPath<UMI3DCollabLoadingParameters>(path);

            foreach (var format in loadingParam.supportedformats)
                Debug.Log(format);

            if (loadingParam.supportedformats.Contains(UMI3DAssetFormat.unity_android_urp))
                found = true;
        }

        if (!found)
            throw new BuildFailedException("Impossible to build with " + UMI3DAssetFormat.unity_android_urp + " format not added to supported format");
    }
}
