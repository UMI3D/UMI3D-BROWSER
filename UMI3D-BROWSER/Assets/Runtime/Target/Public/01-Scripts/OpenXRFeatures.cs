/*
Copyright 2019 - 2025 Inetum

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

using System;
using System.Collections.Generic;
using umi3d.common.core.target;

namespace umi3d.browserRuntime.target
{
    public static class OpenXRFeatures 
    {
        public static IReadOnlyList<Feature> allCases => _allCases.Value;
        static Lazy<Feature[]> _allCases = new(() =>
        {
            List<Feature> list = new List<Feature>();
            list.AddRange(_allMetaQuestCases.Value);
            list.AddRange(_allPicoCases.Value);
            list.AddRange(_allViveCases.Value);
            return list.ToArray();
        });

        public static IReadOnlyList<Feature> allMetaQuestCases => _allMetaQuestCases.Value;
        static Lazy<Feature[]> _allMetaQuestCases = new(() =>
        {
            return new[] { MetaQuestSupport, MetaQuestARAnchors, MetaQuestARCamera, MetaQuestARPlaneDetection, MetaQuestARRaycasts, MetaQuestARSession, MetaQuestDisplayUtilities, MetaQuestTouchProController, OculusTouchController };
        });

        public static IReadOnlyList<Feature> allPicoCases => _allPicoCases.Value;
        static Lazy<Feature[]> _allPicoCases = new(() =>
        {
            return new[] { PICOSupport, PICOCompositionLayerSecureContent, PICODisplayRefreshRate, PICOFoveation, PICOPassthrough, PICOOpenXRFeatures, PICOPerformanceSettings, PICO4TouchController, PICONeo3TouchController };
        });

        public static IReadOnlyList<Feature> allViveCases => _allViveCases.Value;
        static Lazy<Feature[]> _allViveCases = new(() =>
        {
            return new[] { VIVEXRCompositionLayer, VIVEXRCompositionLayerColorScaleBias, VIVEXRCompositionLayerCylinder, VIVEXRFacialTracking, VIVEXRFoveation, VIVEXRHandTracking, VIVEXRSupport, VIVEFocus3Controller };
        });

        public static IReadOnlyList<Feature> allARCases => _allARCases.Value;
        static Lazy<Feature[]> _allARCases = new(() =>
        {
            return new[] {
                MetaQuestARSession, MetaQuestARCamera, MetaQuestARPlaneDetection, MetaQuestARAnchors, MetaQuestARRaycasts,
                PICOPassthrough
            };
        });

        // Meta features.
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.MetaQuestSupport.MetaQuestFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestSupport = new Feature("Meta Quest Support", "com.unity.openxr.feature.metaquest");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.ARAnchorFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestARAnchors = new Feature("Meta Quest: AR Anchors", "com.unity.openxr.feature.arfoundation-meta-anchor");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.ARCameraFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestARCamera = new Feature("Meta Quest: AR Camera (Passthrough)", "com.unity.openxr.feature.arfoundation-meta-camera");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.ARPlaneFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestARPlaneDetection = new Feature("Meta Quest: AR Plane Detection", "com.unity.openxr.feature.arfoundation-meta-plane");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.ARRaycastFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestARRaycasts = new Feature("Meta Quest: AR Raycasts", "com.unity.openxr.feature.arfoundation-meta-raycast");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.ARSessionFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestARSession = new Feature("Meta Quest: AR Session", "com.unity.openxr.feature.arfoundation-meta-session");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Meta.DisplayUtilitiesFeature.featureId"/>
        /// </summary>
        public static readonly Feature MetaQuestDisplayUtilities = new Feature("Meta Quest: Display Utilities", "com.unity.openxr.feature.meta-display-utilities");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Interactions.MetaQuestTouchProControllerProfile.featureId"/>.
        /// </summary>
        public static readonly Feature MetaQuestTouchProController = new Feature("Meta Quest Touch Pro Controller Profile", "com.unity.openxr.feature.input.metaquestpro");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Interactions.OculusTouchControllerProfile.featureId"/>.
        /// </summary>
        public static readonly Feature OculusTouchController = new Feature("Oculus Touch Controller Profile", "com.unity.openxr.feature.input.oculustouch");

        // Pico features.
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.PICOFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICOSupport = new Feature("PICO Support", "com.unity.openxr.feature.pico");
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.LayerSecureContentFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICOCompositionLayerSecureContent = new Feature("OpenXR Composition Layer Secure Content", "com.pico.openxr.feature.LayerSecureContent");
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.DisplayRefreshRateFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICODisplayRefreshRate = new Feature("OpenXR Display Refresh Rate", "com.pico.openxr.feature.refreshrate");
        /// <summary>
        /// <see cref="FoveationFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICOFoveation = new Feature("OpenXR Foveation", "com.pico.openxr.feature.foveation");
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.PassthroughFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICOPassthrough = new Feature("OpenXR Passthrough", "com.pico.openxr.feature.passthrough");
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.OpenXRExtensions.featureId"/>.
        /// </summary>
        public static readonly Feature PICOOpenXRFeatures = new Feature("PICO OpenXR Features", "com.unity.openxr.pico.features");
        /// <summary>
        /// <see cref="Unity.XR.OpenXR.Features.PICOSupport.PerformanceSettingsFeature.featureId"/>.
        /// </summary>
        public static readonly Feature PICOPerformanceSettings = new Feature("OpenXR Performance Settings", "com.pico.openxr.feature.performancesettings");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Interactions.PICO4ControllerProfile.featureId"/>.
        /// </summary>
        public static readonly Feature PICO4TouchController = new Feature("PICO4 Touch Controller Profile", "com.unity.openxr.feature.input.PICO4touch");
        /// <summary>
        /// <see cref="UnityEngine.XR.OpenXR.Features.Interactions.PICONeo3ControllerProfile.featureId"/>.
        /// </summary>
        public static readonly Feature PICONeo3TouchController = new Feature("PICO Neo3 Touch Controller Profile", "com.unity.openxr.feature.input.PICONeo3touch");

        // Vive features.
        /// <summary>
        /// <see cref="Wave.OpenXR.CompositionLayer.ViveCompositionLayer.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRCompositionLayer = new Feature("VIVE XR Composition Layer", "vive.wave.openxr.feature.compositionlayer");
        /// <summary>
        /// <see cref="Wave.OpenXR.CompositionLayer.ViveCompositionLayerColorScaleBias.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRCompositionLayerColorScaleBias = new Feature("VIVE XR Composition Layer (Color Scale Bias)", "vive.wave.openxr.feature.compositionlayer.colorscalebias");
        /// <summary>
        /// <see cref="Wave.OpenXR.CompositionLayer.ViveCompositionLayerCylinder.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRCompositionLayerCylinder = new Feature("VIVE XR Composition Layer (Cylinder)", "vive.wave.openxr.feature.compositionlayer.cylinder");
        /// <summary>
        /// <see cref="Wave.OpenXR.FacialTracking.ViveFacialTracking.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRFacialTracking = new Feature("VIVE XR Facial Tracking", "vive.wave.openxr.feature.facial.tracking");
        /// <summary>
        /// <see cref="Wave.OpenXR.ViveFoveation.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRFoveation = new Feature("VIVE XR Foveation", "vive.wave.openxr.feature.foveation");
        /// <summary>
        /// <see cref="Wave.OpenXR.Hand.ViveHandTracking.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRHandTracking = new Feature("VIVE XR Hand Tracking", "vive.wave.openxr.feature.hand.tracking");
        /// <summary>
        /// <see cref="Wave.OpenXR.VIVEFocus3Feature.featureId"/>
        /// </summary>
        public static readonly Feature VIVEXRSupport = new Feature("VIVE XR Support", "com.unity.openxr.feature.vivefocus3");
        /// <summary>
        /// <see cref="Wave.OpenXR.VIVEFocus3Profile.featureId"/>
        /// </summary>
        public static readonly Feature VIVEFocus3Controller = new Feature("VIVE Focus 3 Controller Interaction", "vive.wave.openxr.feature.focus3controller");
    }
}