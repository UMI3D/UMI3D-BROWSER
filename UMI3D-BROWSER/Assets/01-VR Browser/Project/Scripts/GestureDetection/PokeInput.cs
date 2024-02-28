using System;
using System.Collections;
using System.Collections.Generic;

using umi3d.cdk.interaction;
using umi3d.picoBrowser;

using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.interactions.selection;
using umi3dVRBrowsersBase.ui;

using UnityEngine;
using UnityEngine.UI;

public class VRPokeInputObserver : MonoBehaviour
{
    public VRSelectionManager selectionManager;

    public Collider pokeCollider;

    [SerializeField]
    private ActionType actionType;
    public ActionType ActionType => actionType;

    [SerializeField]
    private float pokeFrequency = 0.2f;

    public event System.Action Poked;
    public event System.Action Unpoked;

    private void Start()
    {
        pokeCollider = pokeCollider == null ? GetComponent<Collider>() : pokeCollider;

        selectionManager.selectableSelector.selectionEvent.AddListener((e) => AddSelected(e.selectedObject.gameObject));
        selectionManager.selectableSelector.deselectionEvent.AddListener((e) => RemoveSelected(e.selectedObject.gameObject));

        selectionManager.interactableSelector.selectionEvent.AddListener((e) => AddSelected(e.selectedObject.gameObject));
        selectionManager.interactableSelector.deselectionEvent.AddListener((e) => RemoveSelected(e.selectedObject.gameObject));

        selectionManager.elementSelector.selectionEvent.AddListener((e) => AddSelected(e.selectedObject.gameObject));
        selectionManager.elementSelector.deselectionEvent.AddListener((e) => RemoveSelected(e.selectedObject.gameObject));
    }

    private HashSet<Collider> containedColliders = new();

    private void OnTriggerEnter(Collider other)
    {
        if(containedColliders.Contains(other))
            return;

        Debug.Log($"Add collider of object {other.name}");
        containedColliders.Add(other);
        TryPoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!containedColliders.Contains(other))
            return;

        Debug.Log($"Remove collider of object {other.name}");

        containedColliders.Remove(other);
    }

    public HashSet<GameObject> selectedObjects = new HashSet<GameObject>();

    private void AddSelected(GameObject selectedGo)
    {
        selectedObjects.Add(selectedGo);
        TryPoke(selectedGo);
    }

    private void RemoveSelected(GameObject selectedGo)
    {
        selectedObjects.Remove(selectedGo);
    }

    private void TryPoke(GameObject go)
    {
        if (!canPoke)
            return;

        if (!selectedObjects.Contains(go))
            return;

        Collider collider = go.GetComponentInChildren<Collider>();
        if (!containedColliders.Contains(collider))
            return;

        Poked?.Invoke();
        Debug.Log($"POKE {go.name}");
        canPoke = false; // don't spam pokes
        StartCoroutine(WaitBeforeNextPokeCoroutine());
    }

    private bool canPoke = true;

    private IEnumerator WaitBeforeNextPokeCoroutine()
    {
        yield return new WaitForSeconds(pokeFrequency);
        canPoke = true;
        Unpoked?.Invoke();
    }
}
