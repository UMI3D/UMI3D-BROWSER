using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gestures", menuName = "Gestures")]
public class GestureSet : ScriptableObject
{
    [SerializeField] private List<Gesture> _gestures;
    public List<Gesture> Gestures => _gestures;
}
