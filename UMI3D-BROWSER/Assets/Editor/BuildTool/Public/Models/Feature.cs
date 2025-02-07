/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections.Generic;
using Unity.XR.OpenXR.Features.PICOSupport;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using Wave.OpenXR;
using Wave.OpenXR.CompositionLayer;
using Wave.OpenXR.FacialTracking;
using Wave.OpenXR.Hand;

namespace umi3d.browserEditor.BuildTool
{
    public struct Feature
    {
        public string name;

        public string id;

        public Feature(string name, string id)
        {
            this.name = name;
            this.id = id;
        }

        static Feature[] _allCases
        {
            get
            {
                List<Feature> list = new List<Feature>();
                list.AddRange(allMetaQuestCases);
                list.AddRange(allPicoCases);
                list.AddRange(allViveCases);
                return list.ToArray();
            }
        }
        public static Feature[] allCases = _allCases;

        public static Feature[] allMetaQuestCases = { MetaQuestSupport, MetaQuestARAnchors, MetaQuestARCamera, MetaQuestARPlaneDetection, MetaQuestARRaycasts, MetaQuestARSession, MetaQuestDisplayUtilities, MetaQuestTouchProController, OculusTouchController };

        public static Feature[] allPicoCases = { PICOSupport, PICOCompositionLayerSecureContent, PICODisplayRefreshRate, PICOFoveation, PICOPassthrough, PICOOpenXRFeatures, PICOPerformanceSettings, PICO4TouchController, PICONeo3TouchController };

        public static Feature[] allViveCases = { VIVEXRCompositionLayer, VIVEXRCompositionLayerColorScaleBias, VIVEXRCompositionLayerCylinder, VIVEXRFacialTracking, VIVEXRFoveation, VIVEXRHandTracking, VIVEXRSupport, VIVEFocus3Controller };

        // Meta features.
        public static Feature MetaQuestSupport = new Feature("Meta Quest Support", MetaQuestFeature.featureId);
        public static Feature MetaQuestARAnchors = new Feature("Meta Quest: AR Anchors", ARAnchorFeature.featureId);
        public static Feature MetaQuestARCamera = new Feature("Meta Quest: AR Camera (Passthrough)", ARCameraFeature.featureId);
        public static Feature MetaQuestARPlaneDetection = new Feature("Meta Quest: AR Plane Detection", ARPlaneFeature.featureId);
        public static Feature MetaQuestARRaycasts = new Feature("Meta Quest: AR Raycasts", ARRaycastFeature.featureId);
        public static Feature MetaQuestARSession = new Feature("Meta Quest: AR Session", ARSessionFeature.featureId);
        public static Feature MetaQuestDisplayUtilities = new Feature("Meta Quest: Display Utilities", DisplayUtilitiesFeature.featureId);
        public static Feature MetaQuestTouchProController = new Feature("Meta Quest Touch Pro Controller Profile", MetaQuestTouchProControllerProfile.featureId);
        public static Feature OculusTouchController = new Feature("Oculus Touch Controller Profile", OculusTouchControllerProfile.featureId);

        // Pico features.
        public static Feature PICOSupport = new Feature("PICO Support", PICOFeature.featureId);
        public static Feature PICOCompositionLayerSecureContent = new Feature("OpenXR Composition Layer Secure Content", LayerSecureContentFeature.featureId);
        public static Feature PICODisplayRefreshRate = new Feature("OpenXR Display Refresh Rate", DisplayRefreshRateFeature.featureId);
        public static Feature PICOFoveation = new Feature("OpenXR Foveation", FoveationFeature.featureId);
        public static Feature PICOPassthrough = new Feature("OpenXR Passthrough", PassthroughFeature.featureId);
        public static Feature PICOOpenXRFeatures = new Feature("PICO OpenXR Features", OpenXRExtensions.featureId);
        public static Feature PICOPerformanceSettings = new Feature("OpenXR Performance Settings", PerformanceSettingsFeature.featureId);
        public static Feature PICO4TouchController = new Feature("PICO4 Touch Controller Profile", PICO4ControllerProfile.featureId);
        public static Feature PICONeo3TouchController = new Feature("PICO Neo3 Touch Controller Profile", PICONeo3ControllerProfile.featureId);

        // Vive features.
        public static Feature VIVEXRCompositionLayer = new Feature("VIVE XR Composition Layer", ViveCompositionLayer.featureId);
        public static Feature VIVEXRCompositionLayerColorScaleBias = new Feature("VIVE XR Composition Layer (Color Scale Bias)", ViveCompositionLayerColorScaleBias.featureId);
        public static Feature VIVEXRCompositionLayerCylinder = new Feature("VIVE XR Composition Layer (Cylinder)", ViveCompositionLayerCylinder.featureId);
        public static Feature VIVEXRFacialTracking = new Feature("VIVE XR Facial Tracking", ViveFacialTracking.featureId);
        public static Feature VIVEXRFoveation = new Feature("VIVE XR Foveation", ViveFoveation.featureId);
        public static Feature VIVEXRHandTracking = new Feature("VIVE XR Hand Tracking", ViveHandTracking.featureId);
        public static Feature VIVEXRSupport = new Feature("VIVE XR Support", VIVEFocus3Feature.featureId);
        public static Feature VIVEFocus3Controller = new Feature("VIVE Focus 3 Controller Interaction", VIVEFocus3Profile.featureId);
    }
}
