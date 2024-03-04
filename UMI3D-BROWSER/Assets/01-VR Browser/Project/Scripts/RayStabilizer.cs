using umi3dVRBrowsersBase.interactions.selection.cursor;
using UnityEngine;

/// <summary>
/// Moving average filter Algorithm apply to a transform
/// </summary>
public class RayStabilizer : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private RayCursor m_RayCursor;
    [SerializeField] private FilterAlgoRotation m_Filter;

    protected void OnEnable()
    {
        m_Filter.Initialize((m_Target.forward, m_Target.up));
    }

    protected void Update()
    {
        (Vector3 forward, Vector3 up) = m_Filter.Filter((m_Target.forward, m_Target.up), Time.deltaTime);

        transform.SetPositionAndRotation(m_Target.position, Quaternion.LookRotation(forward, up));
    }
}