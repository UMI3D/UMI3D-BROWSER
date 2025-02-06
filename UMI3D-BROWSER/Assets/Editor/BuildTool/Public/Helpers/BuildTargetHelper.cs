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

using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.browserRuntime.conditionalCompilation;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.Management;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    /// <summary>
    /// Let user switch target.<br/>
    /// If the target change then a new compilation can be needed.
    /// </summary>
    public class BuildTargetHelper 
    {
        /// <summary>
        /// The default instance of <see cref="BuildTargetHelper"/>.<br/>
        /// <b>Warning:</b> You first have to initialized this instance by calling <see cref="Init(E_Target)"/>.
        /// </summary>
        public static BuildTargetHelper @default
        {
            get
            {
                if (!hasBeenInitialized)
                {
                    UnityEngine.Debug.LogError($"[BuildTargetHelper] Error: you must call BuildTargetHelper.Init before using this property.");
                    throw new Exception("Not initialized");
                }

                return _default.Value;
            }
        }
        static readonly Lazy<BuildTargetHelper> _default = new(() => new());
        internal static bool hasBeenInitialized = false;

        public E_Target target { get; private set; }
        public ITargetDelegate @delegate;
        internal IUnityTargetDelegate unityDelegate;

        public static void Init(E_Target target)
        {
            if (hasBeenInitialized)
            {
                UnityEngine.Debug.LogWarning($"[BuildTargetHelper] Warning: has already been initialized.");
                return;
            }

            _default.Value.target = target;
            _default.Value.unityDelegate = new UnityTargetDelegate();
            hasBeenInitialized = true;
        }

        /// <summary>
        /// Switch target.<br/>
        /// Update compilation symbols and change build target.<br/>
        /// </summary>
        /// <param name="target"></param>
        public void SwitchTarget(E_Target target)
        {
            // Build target.
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    ChangeBuildTarget(
                        BuildTargetGroup.Android, 
                        BuildTarget.Android
                    );
                    break;

                case E_Target.SteamVR:
                    ChangeBuildTarget(
                        BuildTargetGroup.Standalone,
                        BuildTarget.StandaloneWindows64
                    );
                    break;

                case E_Target.Windows:
                    ChangeBuildTarget(
                        BuildTargetGroup.Standalone,
                        BuildTarget.StandaloneWindows64
                    );
                    break;

                default:
                    break;
            }

            // Symbols
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                case E_Target.SteamVR:
                    ChangeDeviceConditionalCompilation(MultiDevice.XR);
                    break;

                case E_Target.Windows:
                    ChangeDeviceConditionalCompilation(MultiDevice.PC);
                    break;

                default:
                    break;
            }

            PluginFeatureHelper.@default.OpenXRFeature(true, new[] { Feature.PICOSupport.id, Feature.PICODisplayRefreshRate.id, Feature.PICOFoveation.id, Feature.PICOPassthrough.id, Feature.PICOCompositionLayerSecureContent.id });

            E_Target oldTarget = this.target;
            this.target = target;
            @delegate.TargetHasChanged(oldTarget, target);
        }

        void ChangeBuildTarget(
            BuildTargetGroup buildTargetGroup, 
            BuildTarget buildTarget
        )
        {
            BuildTarget oldTarget = unityDelegate.GetActiveTarget();
            BuildTargetGroup oldTargetGroup = unityDelegate.GetSelectedTargetGroup();

            if (oldTarget == buildTarget && oldTargetGroup == buildTargetGroup)
            {
                UnityEngine.Debug.Log($"[BuildTargetHelper] Notice: Current target is {buildTarget}");
                return;
            }

            if (!unityDelegate.SwitchTarget(buildTargetGroup, buildTarget))
            {
                UnityEngine.Debug.LogError($"[BuildTargetHelper] Error: Switching target failed.");
                @delegate?.BuildTargetFailedToChange();
            }
            else
            {
                UnityEngine.Debug.Log($"[BuildTargetHelper] Notice: Target switch from {oldTarget} to {buildTarget}");
                @delegate?.BuildTargetHasChanged(buildTargetGroup, buildTarget);
            }
        }

        void ChangeDeviceConditionalCompilation(MultiDevice device)
        {
            BuildTargetGroup targetGroup = unityDelegate.GetSelectedTargetGroup();
            UnityEditor.Build.NamedBuildTarget target = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            string[] currentSymbols = unityDelegate.GetCompilationSymbols(target);

            List<string> newSymbols = new();
            bool hasDeviceSymbolBeenAdded = false;
            bool shouldUpdate = false;
            for (int i = 0; i < currentSymbols.Length; i++)
            {
                string symbol = currentSymbols[i];
                if (symbol.TryGetDeviceFromSymbol(out MultiDevice deviceFromSymbol))
                {
                    if (deviceFromSymbol == device)
                    {
                        newSymbols.Add(symbol);
                        hasDeviceSymbolBeenAdded = true;
                    }
                    else
                    {
                        // else the symbol is not added to the list of new symbols
                        // and will be removed from the current list.
                        shouldUpdate = true;
                    }
                }
                else
                {
                    newSymbols.Add(symbol);
                }
            }
            if (!hasDeviceSymbolBeenAdded)
            {
                newSymbols.Add(device.GetSymbol());
                shouldUpdate = true;
            }

            if (shouldUpdate)
            {
                string[] _newSymbols = newSymbols.ToArray();
                unityDelegate.SetCompilationSymbols(target, _newSymbols);
                @delegate?.SymbolsHaveChanged(currentSymbols, _newSymbols, target);
            }
        }
    }
}