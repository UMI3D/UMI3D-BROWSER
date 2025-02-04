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
using umi3d.browserRuntime.conditionalCompilation;
using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    public static class BuildTargetHelper 
    {
        static umi3d.debug.UMI3DLogger logger 
            = new(mainTag: nameof(BuildTargetHelper));

        public static ITargetDelegate @delegate;

        /// <summary>
        /// Switch target.<br/>
        /// Update compilation symbols and change build target.<br/>
        /// </summary>
        /// <param name="target"></param>
        public static void SwitchTarget(E_Target target)
        {
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    ChangeDeviceConditionalCompilation(
                        MultiDevice.XR, 
                        UnityEditor.Build.NamedBuildTarget.Android
                    );
                    break;

                case E_Target.SteamVR:
                    ChangeDeviceConditionalCompilation(
                        MultiDevice.XR, 
                        UnityEditor.Build.NamedBuildTarget.Standalone
                    );
                    break;

                case E_Target.Windows:
                    ChangeDeviceConditionalCompilation(
                        MultiDevice.PC, 
                        UnityEditor.Build.NamedBuildTarget.Standalone
                    );
                    break;

                default:
                    break;
            }

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
        }

        static void ChangeDeviceConditionalCompilation(
            MultiDevice device,
            UnityEditor.Build.NamedBuildTarget target
        )
        {
            string[] currentSymbols = PlayerSettings
                .GetScriptingDefineSymbols(target)
                .Split(';');

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
                PlayerSettings.SetScriptingDefineSymbols(
                    target,
                    _newSymbols
                );
                @delegate?.SymbolsHaveChanged(currentSymbols, _newSymbols, target);
            }
        }

        static void ChangeBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            BuildTarget oldTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup oldTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (oldTarget == buildTarget && oldTargetGroup == buildTargetGroup)
            {
                logger.Default(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Current target is {buildTarget}"
                );
            }

            bool result = EditorUserBuildSettings.SwitchActiveBuildTarget(
                buildTargetGroup, 
                buildTarget
            );
            // buildTargetGroup is not set correctly with EditorUserBuildSettings.SwitchActiveBuildTarget.
            EditorUserBuildSettings.selectedBuildTargetGroup = buildTargetGroup;

            if (!result)
            {
                logger.Error(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Switching target failed"
                );
                @delegate?.BuildTargetFailedToChange();
            }
            else
            {
                logger.Default(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Target switch from {oldTarget} to {buildTarget}"
                );
                @delegate?.BuildTargetHasChanged(buildTargetGroup, buildTarget);
            }
        }
    }
}