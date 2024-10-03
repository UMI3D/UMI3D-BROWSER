using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NotARectangleButton : MonoBehaviour
{
    private void Awake() => GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
}
