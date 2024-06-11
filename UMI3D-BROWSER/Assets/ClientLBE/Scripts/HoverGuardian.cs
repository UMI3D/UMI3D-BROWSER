using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverGuardian : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Material targetMaterial;

    public string shaderVariableName = "_BlendHeight";
    public float targetValue = 1.5f;
    public float duration = 0.2f;

    // Dictionnaire pour suivre l'�tat de sortie des objets
    private Dictionary<Collider, bool> objectExitStatus = new Dictionary<Collider, bool>();

    private void Start()
    {
        // R�cup�rer le MeshRenderer et le mat�riau original
        meshRenderer = GetComponent<MeshRenderer>();
        targetMaterial = meshRenderer.material;
        targetMaterial.SetFloat(shaderVariableName, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "LeftHand Controller" || other.name == "RightHand Controller")
        {
            if(targetValue == 1)
            {
                StartCoroutine(AnimateShaderVariable(targetValue, 0.0f));
            }
        }
        // Ajouter l'objet au dictionnaire avec l'�tat de sortie initial � false
        objectExitStatus[other] = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // V�rifiez si l'objet est partiellement � l'ext�rieur
        bool isExiting = !IsObjectCompletelyInside(other);

        if (isExiting)
        {
            if (other.name == "LeftHand Controller" || other.name == "RightHand Controller")
            {
                if (!objectExitStatus[other])
                {
                    // Si l'objet commence � sortir
                    StartCoroutine(AnimateShaderVariable(0.0f, targetValue));
                    objectExitStatus[other] = true;
                }
            }
        }
        else
        {
            if (objectExitStatus[other])
            {
                // Si l'objet cesse de sortir
                StartCoroutine(AnimateShaderVariable(targetValue, 0.0f));
                objectExitStatus[other] = false;
            }
        }
    }

    private bool IsObjectCompletelyInside(Collider other)
    {
        // V�rifiez si toutes les parties de l'objet sont toujours dans le volume
        Bounds triggerBounds = GetComponent<Collider>().bounds;
        Bounds objectBounds = other.bounds;

        // V�rifie si une partie des limites de l'objet est en dehors des limites du d�clencheur
        if (!triggerBounds.Contains(objectBounds.min) || !triggerBounds.Contains(objectBounds.max))
        {
            return false;
        }
        return true;
    }

    private IEnumerator AnimateShaderVariable(float startValue, float endValue)
    {
        if (targetMaterial != null && targetMaterial.HasProperty(shaderVariableName))
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float newValue = Mathf.Lerp(startValue, endValue, t);
                targetMaterial.SetFloat(shaderVariableName, newValue);
                yield return null;
            }
            targetMaterial.SetFloat(shaderVariableName, endValue);
        }
        else
        {
            Debug.LogError("Material�: Variable de shader introuvable ou le mat�riau est nul.");
        }
    }
}
