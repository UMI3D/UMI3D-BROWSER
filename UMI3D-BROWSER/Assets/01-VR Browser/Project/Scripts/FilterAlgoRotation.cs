using UnityEngine;

public abstract class FilterAlgoRotation : ScriptableObject
{
    public abstract void Initialize((Vector3 forward, Vector3 up) value);
    public abstract (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) value, float deltaTime);
}