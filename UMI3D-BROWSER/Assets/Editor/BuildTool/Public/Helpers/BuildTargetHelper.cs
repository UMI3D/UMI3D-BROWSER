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
using umi3d.browserRuntime.conditionalCompilation;
using UnityEditor;

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
        BuildTargetHelper() { }

        public E_Target target { get; private set; }
        public ITargetDelegate @delegate;
        internal IUnityTargetDelegate unityDelegate = new UnityTargetDelegate();

        /// <summary>
        /// Initializes the default instance of <see cref="BuildTargetHelper"/> with the current target if it has not been initialized yet.<br/>
        /// If the target has already been initialized, it logs a warning and does not change the target.<br/>
        /// <br/>
        /// <example>
        /// Given the target is not initialized when Init is called then the target is set.<br/>
        /// <code>
        /// BuildTargetHelper.Init(E_Target.SteamVR);
        /// // BuildTargetHelper.@default.target == E_Target.SteamVR
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="target">The target to be set for the build.</param>
        public static void Init(E_Target target)
        {
            if (hasBeenInitialized)
            {
                UnityEngine.Debug.LogWarning($"[BuildTargetHelper] Warning: has already been initialized.");
                return;
            }

            _default.Value.target = target;
            hasBeenInitialized = true;
        }

        /// <summary>
        /// Switches the build target and updates the compilation symbols accordingly.<br/>
        /// <br/>
        /// <example>
        /// Given a target, when switching the target, then the build target and symbols are updated.<br/>
        /// <code>
        /// BuildTargetHelper.@default.SwitchTarget(E_Target.Quest);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="target">The target to switch to.</param>
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
            for (int i = 0; i < (currentSymbols?.Length ?? 0); i++)
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