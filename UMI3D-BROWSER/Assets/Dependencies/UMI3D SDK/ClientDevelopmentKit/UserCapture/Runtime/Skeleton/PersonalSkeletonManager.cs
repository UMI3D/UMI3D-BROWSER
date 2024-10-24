﻿/*
Copyright 2019 - 2023 Inetum

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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common;
using umi3d.common.userCapture.description;
using UnityEngine;

namespace umi3d.cdk.userCapture
{
    /// <summary>
    /// User's skeleton manager.
    /// </summary>
    public class PersonalSkeletonManager : Singleton<PersonalSkeletonManager>, ISkeletonManager, IPersonalSkeletonManager
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.UserCapture;

        /// <inheritdoc/>
        public IPersonalSkeleton PersonalSkeleton
        {
            get
            {
                if (_skeleton == null)
                    _skeleton = GetPersonalSkeleton();
                
                return _skeleton;
            }
            protected set => SetPersonalSkeleton((PersonalSkeleton)value);
        }

        /// <inheritdoc/>
        public UMI3DSkeletonHierarchy StandardHierarchy
        {
            get
            {
                var userCaptureLoadingParameters = (IUMI3DUserCaptureLoadingParameters)environmentLoaderService.AbstractLoadingParameters;
                _standardHierarchy ??= new UMI3DSkeletonHierarchy(userCaptureLoadingParameters.SkeletonHierarchyDefinition, userCaptureLoadingParameters.SkeletonMusclesDefinition);
                return _standardHierarchy;
            }
        }

        public IReadOnlyDictionary<uint, float> BonesAsyncFPS => PersonalSkeleton.BonesAsyncFPS as IReadOnlyDictionary<uint, float>;

        public Vector3 WorldSize => PersonalSkeleton.worldSize;

        private UMI3DSkeletonHierarchy _standardHierarchy;

        private PersonalSkeleton _skeleton;
        public bool IsPersonalSkeletonNull => _skeleton == null;
        private IEnumerator computeRoutine;

        #region Dependency Injection

        private readonly IEnvironmentManager environmentManager;
        private readonly ILoadingManager environmentLoaderService;
        private readonly ILateRoutineService lateRoutineService;

        public PersonalSkeletonManager() : this(UMI3DEnvironmentLoader.Instance, UMI3DEnvironmentLoader.Instance, CoroutineManager.Instance)
        { }

        public PersonalSkeletonManager(IEnvironmentManager environmentManager, ILoadingManager environmentLoaderService, ILateRoutineService lateRoutineService) : base()
        {
            this.environmentManager = environmentManager;
            this.environmentLoaderService = environmentLoaderService;
            this.lateRoutineService = lateRoutineService;
            Init();
        }

        #endregion Dependency Injection

        protected virtual void Init()
        {
            environmentLoaderService.onEnvironmentLoaded.AddListener(delegate
            {
                lateRoutineService.AttachLateRoutine(WaitUntilPersonalSkeletonAvailable(SetPersonalSkeleton));
            });
        }

        private void SetPersonalSkeleton(PersonalSkeleton personalSkeleton)
        {
            if (_skeleton == personalSkeleton)
                return;

            _skeleton = personalSkeleton;
            _skeleton.SkeletonHierarchy = StandardHierarchy;
            _skeleton.Init();

            if (computeRoutine != null)
                StopCompute();

            StartCompute();
        }

        private PersonalSkeleton GetPersonalSkeleton()
        {
            if (_skeleton != null)
                return _skeleton;

            // wait for access to personal skeleton
            if (environmentManager.gameObject == null // EnvironmentLoadingHandler is not in scene
                || environmentManager.gameObject.GetComponentInChildren<PersonalSkeleton>() == null)
                return null;

            SetPersonalSkeleton(environmentManager.gameObject.GetComponentInChildren<PersonalSkeleton>());
            return _skeleton;
        }

        public void StartCompute()
        {
            computeRoutine ??= lateRoutineService.AttachLateRoutine(ComputeCoroutine());
        }

        public void StopCompute()
        {
            if (computeRoutine == null)
                return;

            lateRoutineService.DetachLateRoutine(computeRoutine);
            computeRoutine = null;
        }

        IEnumerator WaitUntilPersonalSkeletonAvailable(Action<PersonalSkeleton> callback)
        {
            while (environmentManager.gameObject == null  // EnvironmentLoadingHandler is not in scene
                || environmentManager.gameObject.GetComponentInChildren<PersonalSkeleton>() == null)
            {
                yield return null;
            }

            callback?.Invoke(environmentManager.gameObject.GetComponentInChildren<PersonalSkeleton>());
        }

        private IEnumerator ComputeCoroutine()
        {
            while (PersonalSkeleton != null)
            {
                _skeleton.Compute();
                yield return null;
            }
            StopCompute();
        }
    }
}