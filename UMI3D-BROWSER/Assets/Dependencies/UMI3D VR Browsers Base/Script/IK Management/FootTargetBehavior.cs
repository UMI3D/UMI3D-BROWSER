/*
Copyright 2019 - 2022 Inetum

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

using umi3d.cdk.userCapture.tracking;
using UnityEngine;

namespace umi3dVRBrowsersBase.ikManagement
{
    public class FootTargetBehavior : MonoBehaviour
    {
        public Transform FollowedAvatarNode;
        public Transform OVRRig;

        public Animator SkeletonAnimator;

        public Tracker LeftTracker;
        public Tracker RightTracker;

        private void Start()
        {
            LeftTracker.isActive = false;
            RightTracker.isActive = false;
        }

        private void Update()
        {
            transform.position = new Vector3(
                FollowedAvatarNode.position.x, 
                OVRRig.position.y, 
                FollowedAvatarNode.position.z
            );
            transform.rotation = FollowedAvatarNode.rotation;
        }

        public void SetFootTargets()
        {
            LeftTracker.transform.position 
                = SkeletonAnimator.GetBoneTransform(
                    HumanBodyBones.LeftFoot
                ).position;
            RightTracker.transform.position 
                = SkeletonAnimator.GetBoneTransform(
                    HumanBodyBones.RightFoot
                ).position;
            LeftTracker.isActive = true;
            RightTracker.isActive = true;
        }
    }
}