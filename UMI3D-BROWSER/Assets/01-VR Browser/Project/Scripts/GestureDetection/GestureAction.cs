using UnityEngine;
using UnityEngine.InputSystem;

public class GestureAction : MonoBehaviour
{
    [SerializeField] private InputActionReference _teleportAction;
    [SerializeField] private GameObject _teleportObject;

    private void Awake()
    {
        _teleportObject.SetActive(false);
    }

    private void OnEnable()
    {
        _teleportAction.action.performed += (context) => _teleportObject.SetActive(true);
        _teleportAction.action.canceled += (context) => _teleportObject.SetActive(false);
    }
}
