using System.Collections;
using System.Collections.Generic;

using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.interactions.selection;

using UnityEngine;

/// <summary>
/// Listen to selection and its own collider to define a poke input.
/// </summary>
public class VRPokeInputObserver : MonoBehaviour
{
    public VRSelectionManager selectionManager;

    public Collider pokeCollider;

    [SerializeField]
    private ActionType actionType;
    public ActionType ActionType => actionType;

    [SerializeField]
    private float pokePeriod = 0.15f;

    [SerializeField]
    private float pokeSamePeriod = 0.3f;

    public event System.Action Poked;
    public event System.Action Unpoked;

    public LayerMask layerToIgnore;

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

    private readonly HashSet<Collider> containedColliders = new();

    private void OnTriggerEnter(Collider other)
    {
        if (containedColliders.Contains(other))
            return;

        if (other.gameObject.layer == layerToIgnore)
            return;

        containedColliders.Add(other);

        TryPoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!containedColliders.Contains(other))
            return;

        containedColliders.Remove(other);
    }

    private void FixedUpdate()
    {
        if (selectionManager.LastSelectedInfo == null || !selectionManager.LastSelectedInfo.hasBeenSelected)
        {
            selectedObjects.Clear();
        }
    }

    public HashSet<GameObject> selectedObjects = new();

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

        if (!canPokeSame && go == lastPoked)
            return;

        if (!selectedObjects.Contains(go))
            return;

        Collider collider = go.GetComponentInChildren<Collider>();
        if (!containedColliders.Contains(collider))
            return;

        lastPoked = go;
        Poked?.Invoke();
        canPoke = false; // don't spam pokes
        canPokeSame = false;
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);
        waitCoroutine = StartCoroutine(WaitBeforeNextPokeCoroutine());
    }

    private Coroutine waitCoroutine;
    private bool canPoke = true;
    private bool canPokeSame = true;

    private GameObject lastPoked;

    private IEnumerator WaitBeforeNextPokeCoroutine()
    {
        yield return new WaitForSeconds(pokePeriod);
        ReenablePoke();

        yield return new WaitForSeconds(pokeSamePeriod - pokePeriod);
        ReenableSamePoke();
        waitCoroutine = null;
    }

    private void ReenablePoke()
    {
        canPoke = true;
        Unpoked?.Invoke();
    }

    private void ReenableSamePoke()
    {
        canPokeSame = true;
    }
}
