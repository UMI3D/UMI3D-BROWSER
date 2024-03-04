using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

[CreateAssetMenu(menuName = "FilterAlgo/Average Moving Rotation")]
public class AverageMovingRotation : FilterAlgoRotation
{
    [SerializeField] private int m_WindowSize;

    private Cache<(Vector3 forward, Vector3 up)> m_Cache;

    public override void Initialize((Vector3 forward, Vector3 up) value)
    {
        m_Cache = new(m_WindowSize);
    }

    public override (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) value, float deltaTime)
    {
        m_Cache.Add(value);

        Vector3 newForward = m_Cache[0].forward;
        for (var i = 1; i < m_Cache.Count - 1; i++)
        {
            newForward += m_Cache[i].forward;
        }
        BurstMathUtility.FastSafeDivide(newForward, m_Cache.Count, out var resultForward);

        Vector3 newUp = m_Cache[0].up;
        for (var i = 1; i < m_Cache.Count - 1; i++)
        {
            newUp += m_Cache[i].up;
        }
        BurstMathUtility.FastSafeDivide(newUp, m_Cache.Count, out var resultUp);

        return (resultForward, resultUp);
    }
}