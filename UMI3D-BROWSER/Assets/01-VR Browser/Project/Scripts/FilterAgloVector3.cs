using UnityEngine;

public abstract class FilterAlgoVector3 : ScriptableObject
{
    public abstract void Initialize(Vector3 value);
    public abstract Vector3 Filter(Vector3 value, float deltaTime);
}