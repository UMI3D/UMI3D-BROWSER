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
            [HideInInspector] public Vector3 currentGuardianCenter;
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
            [HideInInspector] public Vector3 currentArcCenter;
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
        /// If <paramref name="animationSpeed"/> is more than 0 then the translation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="animationSpeed"/> is more than 0.<br/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="animationSpeed"></param>
        /// <param name="offset"></param>
        /// <param name="filter"></param>
        void TranslateToward(Vector3 position, float animationSpeed = 0f, Vector3? offset = null, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            // Get the translation from the current position to the new position.
            Vector3 translation = position - mono.transform.localPosition;

            // Filter the translation.
            if (filter.HasValue)
            {
                translation = Vector3.Scale(translation, filter.Value);
            }

            // Get the new position with the filtered translation.
            position = mono.transform.localPosition + translation;

            if (offset == null)
            {
                offset = Vector3.zero;
            }

            if (animationSpeed > 0)
            {
                mono.transform.localPosition = Vector3.Lerp
                (
                    mono.transform.position,
                    position + offset.Value,
                    animationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localPosition = position + offset.Value;
            }
        }

        /// <summary>
        /// Translate of <paramref name="translation"/> with an offset of <paramref name="offset"/> at <see cref="SmoothTranslationSpeed"/> speed.<br/>
        /// <br/>
        /// If <paramref name="animationSpeed"/> is more than 0 then the translation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="animationSpeed"/> is more than 0.<br/>
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="animationSpeed"></param>
        /// <param name="offset"></param>
        /// <param name="filter"></param>
        void Translate(Vector3 translation, float animationSpeed = 0f, Vector3? offset = null, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            // Filter the translation.
            if (filter.HasValue)
            {
                translation = Vector3.Scale(translation, filter.Value);
            }

            // Get the new position with the filtered translation.
            Vector3 position = mono.transform.localPosition + translation;

            if (offset == null)
            {
                offset = Vector3.zero;
            }

            if (animationSpeed > 0)
            {
                mono.transform.localPosition = Vector3.Lerp
                (
                    mono.transform.position,
                    position + offset.Value,
                    animationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localPosition = position + offset.Value;
            }
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotate from the current rotation toward <paramref name="rotation"/>.<br/>
        /// <br/>
        /// If <paramref name="animationSpeed"/> is more than 0 then the rotation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="animationSpeed"/> is more than 0.<br/>
        /// <br/>
        /// The rotation is filtered with <paramref name="filter"/>. A filter of x = 0, y = 1, z = 0 means that the only allowed rotation is on the y axis.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="withAnimation"></param>
        /// <param name="filter"></param>
        void RotateToward(Quaternion endRotation, float animationSpeed = 0f, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            Quaternion _rotation = endRotation * Quaternion.Inverse(mono.transform.localRotation);

            if (filter.HasValue)
            {
                _rotation.eulerAngles = Vector3.Scale(filter.Value, _rotation.eulerAngles);
            }

            // Get the end rotation after adding the rotation to the current rotation.
            endRotation = mono.transform.localRotation * _rotation;

            if (animationSpeed > 0)
            {
                mono.transform.localRotation = Quaternion.Lerp(
                    mono.transform.localRotation,
                    endRotation,
                    animationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localRotation = endRotation;
            }
        }

        /// <summary>
        /// Rotate of <paramref name="rotation"/>.<br/>
        /// <br/>
        /// If <paramref name="animationSpeed"/> is more than 0 then the rotation is animated at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="animationSpeed"/> is more than 0.<br/>
        /// <br/>
        /// The rotation is filtered with <paramref name="filter"/>. A filter of x = 0, y = 1, z = 0 means that the only allowed rotation is on the y axis.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="withAnimation"></param>
        /// <param name="filter"></param>
        void Rotate(Quaternion rotation, float animationSpeed = 0f, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            if (filter.HasValue)
            {
                rotation.eulerAngles = Vector3.Scale(filter.Value, rotation.eulerAngles);
            }

            // Get the end rotation after adding the rotation to the current rotation.
            Quaternion endRotation = mono.transform.localRotation * rotation;

            if (animationSpeed > 0)
            {
                mono.transform.localRotation = Quaternion.Lerp(
                    mono.transform.localRotation,
                    endRotation,
                    animationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localRotation = endRotation;
            }
        }

        #endregion
    }
}