using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildTool
{
    public static class BuildStaticNames
    {
        public const string LOADER_OPEN_XR = "UnityEngine.XR.OpenXR.OpenXRLoader";
        public const string LOADER_OCULUS = "Unity.XR.Oculus.OculusLoader";
        public const string LOADER_OPEN_VR = "Unity.XR.OpenVR.OpenVRLoader";
        public const string LOADER_PICO = "Unity.XR.PXR.PXR_Loader";
        public const string LOADER_WAVE_XR = "Wave.XR.Loader.WaveXRLoader";

        public const string FEATURE_META_QUEST = "com.unity.openxr.feature.metaquest";
        public const string FEATURE_PICO_SUPPORT = "com.unity.openxr.feature.pico";
        public const string FEATURE_PICO_OPENXR = "com.unity.openxr.pico.features";
        public const string FEATURE_VIVE_SUPPORT = "com.unity.openxr.feature.vivefocus3";

        public const string INPUT_OCULUS_TOUCH = "com.unity.openxr.feature.input.oculustouch";
        public const string INPUT_METAQUEST_PRO = "com.unity.openxr.feature.input.metaquestpro";
        public const string INPUT_PICO4_TOUCH = "com.unity.openxr.feature.input.PICO4touch";
        public const string INPUT_PICONeo3_TOUCH = "com.unity.openxr.feature.input.PICONeo3touch";
        public const string INPUT_VIVEFocus3 = "vive.wave.openxr.feature.focus3controller";
    }
}

