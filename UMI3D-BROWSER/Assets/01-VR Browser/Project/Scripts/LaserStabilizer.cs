using umi3d.runtimeBrowser.filter;
using umi3dBrowsers.connection;
using umi3dVRBrowsersBase.connection;

using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stabilize and hand tracking ray.
/// </summary>
/// Use an Aim Point that describe where the ray should get through and a Ray origin point
/// that is a point on the arm of the user, between the shoulder and the wrist.
/// Its exact position is determined by a coefficient. 
/// At 0 the origin in on the shoulder. It ends up with a very stable ray.
/// At 1 it is on the wrist. It ends up with a ray that is very responsive to a hand rotation.
public class LaserStabilizer : MonoBehaviour
{
    [Header("Parameters"), SerializeField]
    private Transform shoulderTransform;

    [SerializeField]
    private Transform elbowTransform;

    [SerializeField]
    private Transform wristTransform;

    [SerializeField, Tooltip("Define the position of the ray origin along the arm.\n" +
        "At 0 the origin in on the shoulder. It ends up with a very stable ray.\n" +
        "At 1 it is on the wrist. It ends up with a ray that is very responsive to a hand rotation.")]
    private float intentCoefficient;

    [Header("Hand"), SerializeField, Tooltip("Where the ray should get through.")]
    private Transform aimPoint;

    [SerializeField, Tooltip("Transform rotated by the stabilizer.")]
    private Transform laserOrigin;

    [Header("Pinch stabilization"), SerializeField]
    private Transform thumbTip;

    [SerializeField]
    private Transform indexTip;

    [SerializeField]
    private OneEuroTransformFilter filter;

    /// <summary>
    /// When tips are close than this distance, the ray start to be stronger stabilized.
    /// </summary>
    /// Default value was found empirically.
    [SerializeField, Tooltip("When tips are close than this distance, the ray start to be stronger stabilized.")]
    private float pinchStabilizationThreshold = 0.025f;

    [Header("Starting event provider"), SerializeField, Tooltip("Provide the event that the skeleton is ready.")]
    private SetUpSkeleton setUpSkeleton;

    private bool isRunning = false;

    private Vector3 rayOriginPosition;
    private Vector3 rayDirection;

    void Start()
    {
        Assert.IsNotNull(shoulderTransform);
        Assert.IsNotNull(elbowTransform);
        Assert.IsNotNull(wristTransform);
        Assert.IsNotNull(laserOrigin);
        Assert.IsNotNull(filter);

        filter.IsRunning = false;

        setUpSkeleton = setUpSkeleton == null ? UnityEngine.Object.FindObjectOfType<SetUpSkeleton>() : setUpSkeleton;
        setUpSkeleton.SetupDone += () => { isRunning = true; };
    }

    void Update()
    {
        if (!isRunning)
            return;

        float pinchStrength = (indexTip.position - thumbTip.position).magnitude / pinchStabilizationThreshold;

        if (pinchStrength < 1)
        {
            if (!filter.IsRunning)
            {
                filter.ResetFilter();
                filter.IsRunning = true;
            }
        }
        else
        {
            if (filter.IsRunning)
                filter.IsRunning = false;
        }

        rayOriginPosition = ComputeRayOriginPosition(intentCoefficient);

        rayDirection = (aimPoint.position - rayOriginPosition).normalized;

        laserOrigin.rotation = Quaternion.FromToRotation(Vector3.up, rayDirection);
    }

    private Vector3 ComputeRayOriginPosition(float coeff)
    {
        if (coeff < 0.5)
            return Vector3.Lerp(shoulderTransform.position, elbowTransform.position, coeff / 0.5f);
        else
            return Vector3.Lerp(elbowTransform.position, wristTransform.position, (coeff - 0.5f) / 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isEditor)
            return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(shoulderTransform.position, 0.0125f);
        Gizmos.DrawWireSphere(wristTransform.position, 0.0125f);


        Gizmos.DrawLine(shoulderTransform.position, elbowTransform.position);
        Gizmos.DrawLine(elbowTransform.position, wristTransform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(aimPoint.position, 0.0075f);
        Gizmos.DrawWireSphere(rayOriginPosition, 0.0075f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOriginPosition, rayDirection);
    }
}

