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

using umi3d.cdk.binding;
using umi3d.cdk.userCapture.tracking;
using umi3d.common;
using umi3d.common.core;
using umi3d.common.userCapture.binding;
using UnityEngine;

namespace umi3d.cdk.userCapture.binding
{
    /// <summary>
    /// Client support for bone binding.
    /// </summary>
    public class BoneBinding : AbstractSimpleBinding
    {
        private const DebugScope DEBUG_SCOPE = DebugScope.CDK | DebugScope.UserCapture;

        public BoneBinding(BoneBindingDataDto dto, Transform boundTransform, ISkeleton skeleton) : base(dto, boundTransform)
        {
            this.skeleton = skeleton;
            this.UserId = dto.userId;
            this.BoneType = dto.boneType;
            this.BindToController = dto.bindToController;

            BoneBindingDataDto = (BoneBindingDataDto)SimpleBindingData;
        }

        #region DTO Access

        protected BoneBindingDataDto BoneBindingDataDto { get; }

        /// <summary>
        /// See <see cref="BoneBindingDataDto.userId"/>.
        /// </summary>
        public ulong UserId { get; }

        /// <summary>
        /// See <see cref="BoneBindingDataDto.boneType"/>.
        /// </summary>
        public uint BoneType { get; }

        /// <summary>
        /// See <see cref="BoneBindingDataDto.bindToController"/>.
        /// </summary>
        public bool BindToController { get; }

        #endregion DTO Access

        /// <summary>
        /// Skeleton on which the binding is applied.
        /// </summary>
        protected ISkeleton skeleton;

        /// <inheritdoc/>
        public override void Apply(out bool success)
        {
            if (boundTransform == null) // node is destroyed
            {
                UMI3DLogger.LogWarning($"Bound transform is null. It may have been deleted without removing the binding first.", DEBUG_SCOPE);
                success = false;
                return;
            }

            if (!hasStartedToBeApplied)
                Start();

            ITransformation parentBone = null;

            if (BindToController && skeleton.TrackedSubskeleton.Controllers.TryGetValue(BoneType, out IController controller))
            {
                parentBone = controller.transformation;
            }
            else if (skeleton.Bones.TryGetValue(BoneType, out var boneTransformation))
            {
                parentBone = boneTransformation;
            }

            if (parentBone == null)
            {
                UMI3DLogger.LogWarning($"Bone transform from bone {BoneType} is null. It may have been deleted without removing the binding first.", DEBUG_SCOPE);
                success = false;
                return;
            }

            Compute(parentBone);
            success = true;
        }
    }
}