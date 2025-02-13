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

using System.Collections.Generic;
using System.Linq;
using umi3d.common.core.target;
using UnityEditor;
using UnityEngine;
using DataCreation = umi3d.browserEditor.BuildTool.UMI3DBuildToolDataCreation;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "BuildToolData", menuName = "UMI3D/browser/BuildToolData", order = 1)]
    public class BuildToolData : ScriptableObject
    {
        static BuildToolData _default;
        public static BuildToolData @default
        {
            get
            {
                if (_default == null)
                {
                    DataCreation.GetPath();
                    DataCreation.CreateExcludedFolderIfNecessary();
                    DataCreation.GetFiles();
                    _default = DataCreation.GetSO<BuildToolData>("BuildToolData");
                }
                return _default;
            }
        }
        BuildToolData() {}

        public List<IBuildToolDataDelegate> delegates = new();

        [SerializeField, HideInInspector] View _currentSelectedView = View.InfoView;
        public View currentSelectedView
        {
            get => _currentSelectedView;
            set
            {
                _currentSelectedView = value;
                Save();
                delegates.ForEach(@delegate =>
                {
                    @delegate.CurrentSelectedViewHasChanged(value);
                });
            }
        }

        [SerializeField, HideInInspector] Platform _currentPlatform = Platform.pc;
        public Platform currentPlatform
        {
            get => _currentPlatform;
            set
            {
                _currentPlatform = value;
                Save();
                // TODO: UPDATE
            }
        }

        [SerializeField, HideInInspector] ReleaseCycle _currentReleaseCycle = ReleaseCycle.production;
        public ReleaseCycle currentReleaseCycle
        {
            get => _currentReleaseCycle;
            set
            {
                _currentReleaseCycle = value;
                Save();
                // TODO: UPDATE
            }
        }

        [SerializeField, HideInInspector] List<Scene> _scenes = new();
        public IReadOnlyList<Scene> GetScenesFor(Platform platform, ReleaseCycle releaseCycle)
        {
            List<Scene> scenes = new();
            foreach (Scene scene in _scenes)
            {
                if (scene.platforms.Contains(platform) && scene.releaseCycles.Contains(releaseCycle))
                {
                    scenes.Add(scene);
                }
            }

            return scenes;
        }

        [SerializeField, HideInInspector] List<Feature> _features = new();

        [SerializeField, HideInInspector] Platform _currentBuildingPlatform = Platform.pc;
        [SerializeField, HideInInspector] ReleaseCycle _currentBuildingReleaseCycle = ReleaseCycle.production;

        [SerializeField, HideInInspector] Version _currentBrowserVersion;
        public Version currentBrowserVersion
        {
            get => _currentBrowserVersion;
            set
            {
                _currentBrowserVersion = value;
                Save();
                // TODO: UPDATE.
            }
        }

        [SerializeField, HideInInspector] Version _currentSDKVersion;
        public Version currentSDKVersion
        {
            get => _currentSDKVersion;
            set
            {
                _currentSDKVersion = value;
                Save();
                // TODO: UPDATE.
            }
        }

        public OperatingSystem GetOperatingSystemFrom(Platform platform)
        {
            if (platform.Equals(Platform.pc) || platform.Equals(Platform.pc_vr))
            {
                return OperatingSystem.windows;
            }
            else if (platform.Equals(Platform.meta) || platform.Equals(Platform.pico) || platform.Equals(Platform.vive) || platform.Equals(Platform.androidMobile))
            {
                return OperatingSystem.android;
            }
            else
            {
                UnityEngine.Debug.LogError($"[BuildToolData] Error: Unhandled case: {platform}.");
                throw new System.Exception("Unhandled case");
            }
        }

        void Save()
        {
            EditorUtility.SetDirty(this);
        }
    }
}