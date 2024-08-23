/*
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
using System;
using System.Collections;
using UnityEngine;

namespace umi3d.browserRuntime.UX
{
    /// <summary>
    /// This interface allows the classes that implement it to follow a target (translation and rotation).
    /// 
    /// <para>
    /// <exemple>
    /// Follow a target located at position with an offset.
    /// <code>
    ///     (this as IFollowable).Translate(position, offset);
    /// </code>
    /// </exemple>
    /// <exemple>
    /// Follow the rotation of a target but only in the Y axis.
    /// <code>
    ///     (this as IFollowable).Rotate(rotation, Vector3.up);
    /// </code>
    /// </exemple>
    /// </para>
    /// </summary>
    public interface IFollowable
    {
        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can track the translation and rotation speed.
        /// </summary>
        [Serializable]
        public struct FollowSpeedComponents
        {
            [Tooltip("Translation speed of the follower.")]
            public float SmoothTranslationSpeed;

            [Tooltip("Rotation speed of the follower.")]
            public float SmoothRotationSpeed;
        }

        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can track the translation offset distance and rotation.
        /// </summary>
        [Serializable]
        public struct OffsetDistanceRotationComponents
        {
            [Tooltip("Follower's offset distance from the target.")]
            public float OffsetDistance;
            [Tooltip("Follower's offset rotation from the target.")]
            public Vector3 OffsetRotation;

            /// <summary>
            /// Compute the offset position via <see cref="OffsetDistance"/> and <see cref="OffsetRotation"/>.
            /// </summary>
            public Vector3 Offset
            {
                get => Quaternion.Euler(OffsetRotation) * new Vector3(0f, 0f, OffsetDistance);
                set
                {
                    OffsetDistance = value.magnitude;
                    OffsetRotation = Quaternion
                        .FromToRotation(new Vector3(0f, 0f, value.magnitude), value)
                        .eulerAngles;
                }
            }
        }

        /// <summary>
        /// Editor container for organizing purpose.
        /// </summary>
        [Serializable]
        public struct TranslationComponents
        {
            [Tooltip("Translation filter.")]
            public Vector3 filter;
            [Tooltip("Lazy translation guardian sphere radius.")]
            public float lazyGuardianRadius;
            [Tooltip("Lazy translation guardian center.")]
            public Vector3 currentGuardianCenter;
            [Tooltip("Lazy translation coroutine.")]
            [HideInInspector] public Coroutine lazyCoroutine;
        }

        /// <summary>
        /// Editor container for organizing purpose.
        /// </summary>
        [Serializable]
        public struct RotationComponents
        {
            [Tooltip("Rotation filter.")]
            public Vector3 filter;
            [Tooltip("Lazy rotation arc percentage.")]
            public float lazyArcPct;
            [Tooltip("Current center of the lazy rotation arc.")]
            Vector3 currentArcCenter;
            [Tooltip("Lazy rotation coroutine.")]
            [HideInInspector] public Coroutine lazyCoroutine;

            /// <summary>
            /// Current center of the lazy rotation arc.
            /// </summary>
            public Quaternion CurrentArcCenter
            {
                get => Quaternion.Euler(currentArcCenter);
                set => currentArcCenter = value.eulerAngles;
            }
        }

        /// <summary>
        /// The speed of the translate animation.
        /// </summary>
        float SmoothTranslationSpeed { get; set; }
        /// <summary>
        /// The speed of the rotate animation.
        /// </summary>
        float SmoothRotationSpeed { get; set; }
        
        /// <summary>
        /// Current position of this.
        /// </summary>
        Vector3 CurrentPosition
        {
            get
            {
                if (this is not MonoBehaviour mono) return Vector3.zero;
                return mono.transform.localPosition;
            }
        }

        /// <summary>
        /// Current rotation of this.
        /// </summary>
        Quaternion CurrentRotation
        {
            get
            {
                if (this is not MonoBehaviour mono) return Quaternion.identity;
                return mono.transform.localRotation;
            }
        }

        /// <summary>
        /// Current center of the translation guardian.<br/>
        /// <br/>
        /// When a lazy translation ends this value should be equal to the object position.
        /// </summary>
        Vector3 CurrentGuardianCenter { get; set; }

        /// <summary>
        /// Current center of the lazy rotation arc.<br/>
        /// <br/>
        /// When no rotation is being performed it should correspond to <see cref="CurrentRotation"/>.
        /// </summary>
        Quaternion CurrentArcCenter { get; set; }

        #region Translation

        /// <summary>
        /// Return a distance and a rotation from an offset.
        /// 
        /// <para>
        /// <code>
        ///     ^
        ///     |   x : Offset
        ///     |  /                    
        ///     |a/                     / = Distance
        ///     |/                      a = rotation
        ///     o -------------> 
        /// Translation Target
        /// </code>
        /// </para>
        /// </summary>
        (float distance, Vector3 rotation) OffsetToDistanceRotation(Vector3 offset)
        {
            return (
                offset.magnitude,
                Quaternion
                    .FromToRotation(new Vector3(0f, 0f, offset.magnitude), offset)
                    .eulerAngles
            );
        }

        /// <summary>
        /// Return the offset from a distance and a rotation.
        /// 
        /// <para>
        /// <code>
        ///     ^
        ///     |   x : Offset
        ///     |  /                    
        ///     |a/                     / = Distance
        ///     |/                      a = rotation
        ///     o -------------> 
        /// Translation Target
        /// </code>
        /// </para>
        /// </summary>
        Vector3 DistanceRotationToOffset(float distance, Vector3 rotation)
        {
            return Quaternion.Euler(rotation) * new Vector3(0f, 0f, distance);
        }

        /// <summary>
        /// Translate from <see cref="CurrentPosition"/> toward <paramref name="position"/> with an offset of <paramref name="offset"/> at <see cref="SmoothTranslationSpeed"/> speed.<br/>
        /// <br/>
        /// If <paramref name="withAnimation"/> is true then the translation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="withAnimation"/> is true.<br/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="withAnimation"></param>
        /// <param name="offset"></param>
        void Translate(Vector3 position, bool withAnimation = true, Vector3? offset = null, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            // Get the translation from the current position to the new position.
            Vector3 translation = position - CurrentPosition;

            // Filter the translation.
            if (filter.HasValue)
            {
                translation = Vector3.Scale(translation, filter.Value);
            }

            // Get the new position with the filtered translation.
            position = CurrentPosition + translation;

            if (offset == null)
            {
                offset = Vector3.zero;
            }

            if (withAnimation)
            {
                mono.transform.localPosition = Vector3.Lerp
                (
                    CurrentPosition,
                    position + offset.Value,
                    SmoothTranslationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localPosition = position + offset.Value;
            }
        }

        bool ShouldLazyTranslate(ref Vector3 position, float guardianRadius, Vector3? filter = null)
        {
            // Get the translation from the current position to the new position.
            Vector3 translation = position - CurrentPosition;

            // Filter the translation.
            if (filter.HasValue)
            {
                translation = Vector3.Scale(translation, filter.Value);
            }

            // Get the new position with the filtered translation.
            position = CurrentPosition + translation;

            // Calculate the distance between the center of the sphere and the position.
            float distance = Vector3.Distance(position, CurrentGuardianCenter);

            return distance > guardianRadius;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotate from <see cref="CurrentPosition"/> toward <paramref name="rotation"/>.<br/>
        /// <br/>
        /// If <paramref name="withAnimation"/> is true then the rotation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="withAnimation"/> is true.<br/>
        /// <br/>
        /// The rotation is filtered with <paramref name="filter"/>. A filter of x = 0, y = 1, z = 0 means that the only allowed rotation is on the y axis.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="withAnimation"></param>
        /// <param name="filter"></param>
        void Rotate(Quaternion rotation, bool withAnimation = true, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            if (filter.HasValue)
            {
                rotation.eulerAngles = Vector3.Scale(filter.Value, rotation.eulerAngles);
            }

            if (withAnimation)
            {
                mono.transform.localRotation = Quaternion.Lerp(
                    CurrentRotation,
                    rotation,
                    SmoothRotationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localRotation = rotation;
            }
        }

        /// <summary>
        /// Whether the <paramref name="rotation"/> is greater enough.<br/>
        /// <br/>
        /// If after the <paramref name="rotation"/> this object ends up in the current arc then it should not have rotate.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="arcPct"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool ShouldLazyRotate(ref Vector3 rotation, float arcPct, Vector3? filter = null)
        {
            if (filter.HasValue)
            {
                rotation = Vector3.Scale(filter.Value, rotation);
            }

            // Length of the arc.
            float arcLength = 360f * arcPct;
            // Half of the length of a arcLength.
            float halfArcLength = arcLength / 2f;

            bool shouldRotate = false;

            Vector3 currentSequenceCenter = CurrentArcCenter.eulerAngles;

            Vector3 min = new(
                -halfArcLength + currentSequenceCenter.x,
                -halfArcLength + currentSequenceCenter.y,
                -halfArcLength + currentSequenceCenter.z
            );

            Vector3 max = new(
                halfArcLength + currentSequenceCenter.x,
                halfArcLength + currentSequenceCenter.y,
                halfArcLength + currentSequenceCenter.z
            );

            Vector3 middle = new(
               currentSequenceCenter.x,
               currentSequenceCenter.y,
               currentSequenceCenter.z
            );

            for (int i = 0; i < 3; i++)
            {
                if (rotation[i] < min[i] || max[i] < rotation[i])
                {
                    shouldRotate = true;
                }
                else
                {
                    rotation[i] = currentSequenceCenter[i];
                }
            }

            return shouldRotate;
        }

        #endregion

        /// <summary>
        /// Reset the position and translation.
        /// </summary>
        /// <param name="transform"></param>
        void Rest(Transform transform)
        {
            Translate(transform.position, false);
            Rotate(transform.rotation, false);
            CurrentGuardianCenter = transform.position;
            CurrentArcCenter = transform.rotation;
        }
    }
}