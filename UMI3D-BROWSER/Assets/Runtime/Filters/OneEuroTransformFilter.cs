using System.Collections;
using System.Linq;

using umi3d.runtimeBrowser.filter.dependencies;

using UnityEngine;
using UnityEngine.Assertions;

namespace umi3d.runtimeBrowser.filter
{
    /// <summary>
    /// Filter the position and rotation of the current transform according to another.
    /// </summary>
    /// Use the One Euro Filter Unity implementation by Dario Mazzanti.
    /// More information on the One Euro Filter <see cref="https://gery.casiez.net/1euro/"/>.
    public class OneEuroTransformFilter : MonoBehaviour
    {
        public bool IsRunning = true;
        private bool wasRunningLastFrame = false;

        [Header("Input")]
        [SerializeField]
        private Transform sourceTransform;

        [Header("Output")]
        [SerializeField]
        private Transform targetTransform;

        /// <summary>
        /// Duration of the transition in seconds towards the filtered value.
        /// </summary>
        [SerializeField, Tooltip("Duration of the transition in seconds towards the filtered value.")]
        private float transitionDuration = 0.3f;

        private bool isInTransition;

        private OneEuroFilter<Vector3> positionFilter;
        private OneEuroFilter<Quaternion> rotationFilter;

        /// <summary>
        /// Frequency of data passed to the filter.
        /// </summary>
        [Header("Filtering parameters")]
        [Tooltip("Frequency of data passed to the filter")]
        public float freq = 60;

        /// <summary>
        /// Minimum cutoff frequency
        /// </summary>
        /// Decreasing it reduces jitter but increases lag.
        /// Must be strictly positive.
        /// If slow speed jitter is a problem, decrease it.
        [Tooltip("Minimum cutoff frequency.\n" +
            "Decreasing it reduces jitter but increases lag.\n" +
            "Must be strictly positive.")]
        public float freqCutoffMin = 1;

        /// <summary>
        /// Slope of the linear relationship between speed and cutoff frequency.
        /// </summary>
        /// If high speed lag is a problem, increase beta.
        /// First find the right order of magnitude to tune beta,
        /// which depends on the kind of data you manipulate and their units:
        /// do not hesitate to start with values like 0.001 or 0.0001.
        /// You can first multiply and divide beta by factor 10
        /// until you notice an effect on latency when moving quickly.
        [Tooltip("Slope of the linear relationship between speed and cutoff frequency.\n" +
            "If high speed lag is a problem, increase beta.")]
        public float beta = 0;

        private float dCutoff = 1;

        private void Start()
        {
            Assert.IsNotNull(sourceTransform);
            Assert.IsNotNull(targetTransform);

            positionFilter = new(freq, freqCutoffMin, beta, dCutoff);
            rotationFilter = new(freq, freqCutoffMin, beta, dCutoff);
        }

        private void Update()
        {
            if (!IsRunning)
            {
                if (wasRunningLastFrame)
                {
                    wasRunningLastFrame = false;
                    StartCoroutine(EndFiltering());
                    return;
                }
                targetTransform.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
                return;
            }

            if (IsRunning && !wasRunningLastFrame)
            {
                wasRunningLastFrame = true;
                StartCoroutine(StartFiltering());
                return;
            }

            if (isInTransition)
                return;


            var (filteredPosition, filteredRotation) = Filter();
            targetTransform.SetPositionAndRotation(filteredPosition, filteredRotation);
        }

        private IEnumerator StartFiltering()
        {
            isInTransition = true;
            float startTime = Time.time;

            while ((Time.time - startTime) < transitionDuration)
            {
                var (filteredPosition, filteredRotation) = Filter();

                float t = (Time.time - startTime) / transitionDuration;
                targetTransform.SetPositionAndRotation(Vector3.Lerp(sourceTransform.position, filteredPosition, t),
                                                       Quaternion.Slerp(sourceTransform.rotation, filteredRotation, t));
                yield return null;
            }

            var (filteredPositionFinal, filteredRotationFinal) = Filter();
            targetTransform.SetPositionAndRotation(filteredPositionFinal, filteredRotationFinal);

            isInTransition = false;
        }

        private IEnumerator EndFiltering()
        {
            isInTransition = true;
            float startTime = Time.time;

            while ((Time.time - startTime) < transitionDuration)
            {
                var (filteredPosition, filteredRotation) = Filter();

                float t = (Time.time - startTime) / transitionDuration;
                targetTransform.SetPositionAndRotation(Vector3.Lerp(filteredPosition, sourceTransform.position, t),
                                                       Quaternion.Slerp(filteredRotation, sourceTransform.rotation, t));
                yield return null;
            }

            targetTransform.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);

            isInTransition = false;
        }

        private Cache<float> frequencyCache = new(10);

        private (Vector3 position, Quaternion rotation) Filter()
        {
            frequencyCache.Add(1 / Time.deltaTime);
            freq = frequencyCache.Average();

            positionFilter.UpdateParams(freq, freqCutoffMin, beta, dCutoff);
            positionFilter.UpdateParams(freq, freqCutoffMin, beta, dCutoff);

            Vector3 filteredPosition = positionFilter.Filter(sourceTransform.position);
            Quaternion rotation = rotationFilter.Filter(sourceTransform.rotation);

            return (filteredPosition, rotation);
        }

        public void ResetFilter()
        {
            positionFilter = new(freq, freqCutoffMin, beta, dCutoff);
            rotationFilter = new(freq, freqCutoffMin, beta, dCutoff);
        }
    }
}