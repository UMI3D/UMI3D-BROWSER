using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

[CreateAssetMenu(menuName = "FilterAlgo/Average Moving Vector3")]
public class AverageMovingVector3 : FilterAlgoVector3
{
    [SerializeField] private int m_WindowSize;

    private Cache<Vector3> m_Cache;

    public override void Initialize(Vector3 value)
    {
        m_Cache = new(m_WindowSize);
    }

    public override Vector3 Filter(Vector3 value, float deltaTime)
    {
        m_Cache.Add(value);

        Vector3 vec = m_Cache[0];
        for (var i = 1; i < m_Cache.Count; i++)
        {
            vec += m_Cache[i];
        }
        BurstMathUtility.FastSafeDivide(vec, m_Cache.Count, out var result);
        return result;
    }
}