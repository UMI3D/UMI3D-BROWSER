using UnityEngine;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

public class ShowHandsIfTooFar : MonoBehaviour
{
    [SerializeField] private HandVisualizer m_HandVisualizer;
    [SerializeField] private float m_DistanceThreshold = 0.2f;

    [Header("Hands")]
    [SerializeField] private Transform m_LeftWrist;
    [SerializeField] private Transform m_RightWrist;
    [SerializeField] private Transform m_LeftSkeletonWrist;
    [SerializeField] private Transform m_RightSkeletonWrist;

    protected void Update()
    {
        CheckIfShow();
    }

    private void CheckIfShow()
    {
        var areHandFar = (Vector3.Distance(m_LeftWrist.position, m_LeftSkeletonWrist.position) > m_DistanceThreshold
             || Vector3.Distance(m_RightWrist.position, m_RightSkeletonWrist.position) > m_DistanceThreshold);

        if (!m_HandVisualizer.drawMeshes && areHandFar)
            m_HandVisualizer.drawMeshes = true;
        else if (m_HandVisualizer.drawMeshes && !areHandFar)
            m_HandVisualizer.drawMeshes = false;
    }
}
