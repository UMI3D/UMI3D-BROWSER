using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

[CreateAssetMenu(menuName = "FilterAlgo/One Euro Rotation")]
public class OneEuroFilterRotation : FilterAlgoRotation
{
    [SerializeField] private float m_MinCutoff = 0.1f;
    [SerializeField] private float m_Beta = 0.02f;

    private (Vector3 forward, Vector3 up) m_LastRawValue;
    private (Vector3 forward, Vector3 up) m_LastFilteredValue;

    public override void Initialize((Vector3 forward, Vector3 up) value)
    {
        m_LastRawValue = value;
        m_LastFilteredValue = value;
    }

    public override (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) value, float deltaTime) => Filter(value, deltaTime, m_MinCutoff, m_Beta);

    private (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) rawValue, float deltaTime, float minCutoff, float beta)
    {
        var speedForward = (rawValue.forward - m_LastRawValue.forward) / deltaTime;
        var speedUp = (rawValue.up - m_LastRawValue.up) / deltaTime;

        var cutoffs = new Vector3(minCutoff, minCutoff, minCutoff);
        var betaValues = new Vector3(beta, beta, beta);

        var combinedCutoffsForward = cutoffs + Vector3.Scale(betaValues, speedForward);
        var combinedCutoffsUp = cutoffs + Vector3.Scale(betaValues, speedUp);

        BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffsForward, out Vector3 alphaForward);
        BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffsUp, out Vector3 alphaUp);

        var rawFilteredForward = Vector3.Scale(alphaForward, rawValue.forward);
        var rawFilteredUp = Vector3.Scale(alphaUp, rawValue.up);
        var lastFilteredForward = Vector3.Scale(Vector3.one - alphaForward, m_LastFilteredValue.forward);
        var lastFilteredUp = Vector3.Scale(Vector3.one - alphaUp, m_LastFilteredValue.up);

        var filteredValueForward = rawFilteredForward + lastFilteredForward;
        var filteredValueUp = rawFilteredUp + lastFilteredUp;

        m_LastRawValue = rawValue;
        m_LastFilteredValue = (filteredValueForward, filteredValueUp);

        return (filteredValueForward, filteredValueUp);
    }
}