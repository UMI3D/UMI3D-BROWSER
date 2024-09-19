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
using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool Target", menuName = "UMI3D/Build Tools/Build Tool Target")]
    public class UMI3DBuildToolTarget_SO : PersistentScriptableModel
    {
        public event Action selectedTargetsChanged;
        public event Action<E_Target> applyTargetOptionsHandler;
        public event Action<TargetDto[]> buildSelectedTargetHandler;

        public string installer;
        public string license;
        public string buildFolder;
        public E_Target currentTarget;
        public List<TargetDto> targets = new();
        public VisualTreeAsset target_VTA;

        public TargetDto this[int index]
        {
            get
            {
                return targets[index];
            }
            set
            {
                targets[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return targets.Count;
            }
        }

        /// <summary>
        /// All selected targets.
        /// </summary>
        public TargetDto[] SelectedTargets
        {
            get
            {
                return targets.Where(target =>
                {
                    return target.IsTargetEnabled;
                }).ToArray();
            }
        }

        /// <summary>
        /// Get selected targets that correspond to <paramref name="buildTarget"/> and <paramref name="releaseCycle"/>.
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="releaseCycle"></param>
        /// <returns></returns>
        public TargetDto[] GetSelectedTargets(BuildTarget buildTarget, E_ReleaseCycle releaseCycle)
        {
            return targets.Where(target =>
            {
                switch (target.Target)
                {
                    case E_Target.Quest:
                    case E_Target.Focus:
                    case E_Target.Pico:
                        if (buildTarget != BuildTarget.Android)
                        {
                            return false;
                        }
                        break;
                    case E_Target.SteamXR:
                    case E_Target.Windows:
                        if (buildTarget != BuildTarget.StandaloneWindows)
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }

                return target.IsTargetEnabled && target.releaseCycle == releaseCycle;
            }).ToArray();
        }

        #region Installer

        public void UpdateInstaller(string path)
        {
            installer = path;
            Save(editorOnly: true);
        }

        public void BrowseInstaller(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(installer)
                ? Application.dataPath
                : installer;

            string path = EditorUtility.OpenFilePanel(
                    title: "Installer Path",
                    directory,
                    extension: "iss"
                );

            UpdateInstaller(path);
            updateView?.Invoke(path);
        }

        #endregion

        #region License

        public void UpdateLicense(string path)
        {
            license = path;
            Save(editorOnly: true);
        }

        public void BrowseLicense(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(license)
                ? Application.dataPath
                : license;

            string path = EditorUtility.OpenFilePanel(
                    title: "License Path",
                    directory,
                    extension: "txt"
                );

            UpdateLicense(path);
            updateView?.Invoke(path);
        }

        #endregion

        #region BuildFolder

        public void UpdateBuildFolder(int index, string path)
        {
            UpdateTarget(index, _target =>
            {
                _target.BuildFolder = path;
                return _target;
            });
        }

        public void UpdateBuildFolder(string path)
        {
            buildFolder = path;

            for (int index = 0; index < Count; index++)
            {
                UpdateBuildFolder(index, path);
            }

            Save(editorOnly: true);
        }

        public void BrowseBuildFolder(int index, Action<string> updateView)
        {
            var folder = string.IsNullOrEmpty(this[index].BuildFolder)
                ? Application.dataPath
                : this[index].BuildFolder;

            string path = EditorUtility.OpenFolderPanel(
                title: "Build folder",
                folder,
                defaultName: ""
            );

            UpdateBuildFolder(index, path);
            updateView?.Invoke(path);
        }

        public void BrowseBuildFolder(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildFolder)
                ? Application.dataPath
                : buildFolder;

            string path = EditorUtility.OpenFolderPanel(
                    title: "Build Folder",
                    directory,
                    defaultName: ""
                );

            UpdateBuildFolder(path);
            updateView?.Invoke(path);
        }

        #endregion

        public void ApplyCurrentTarget(E_Target target)
        {
            currentTarget = target;
            applyTargetOptionsHandler?.Invoke(target);
            Save(editorOnly: true);
        }

        public void Select(int index, bool isSelected)
        {
            UpdateTarget(index, _target =>
            {
                _target.IsTargetEnabled = isSelected;
                return _target;
            });
        }

        public void ApplyTarget(int index, E_Target target)
        {
            UpdateTarget(index, _target =>
            {
                _target.Target = target;
                return _target;
            });
        }

        public void ApplyReleaseCycle(int index, E_ReleaseCycle releaseCycle)
        {
            UpdateTarget(index, _target =>
            {
                _target.releaseCycle = releaseCycle;
                return _target;
            });
        }

        public void UpdateTarget(int index, Func<TargetDto, TargetDto> change)
        {
            targets[index] = change(targets[index]);
            selectedTargetsChanged?.Invoke();
            Save(editorOnly: true);
        }

        public void BuildSelectedTargets()
        {
            E_ReleaseCycle[] releases = (E_ReleaseCycle[])Enum.GetValues(typeof(E_ReleaseCycle));
            for (int i = releases.Length - 1; i >= 0; i--)
            {
                buildSelectedTargetHandler?.Invoke(
                    GetSelectedTargets(
                        BuildTarget.Android,
                        releases[i]
                    )
                );
            }

            for (int i = releases.Length - 1; i >= 0; i--)
            {
                buildSelectedTargetHandler?.Invoke(
                    GetSelectedTargets(
                        BuildTarget.StandaloneWindows,
                        releases[i]
                    )
                );
            }
        }
    }
}
