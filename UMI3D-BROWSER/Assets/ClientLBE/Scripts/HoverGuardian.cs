using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverGuardian : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    public Material targetMaterial;

    public string shaderVariableName = "_BlendHeight";
    public float targetValue = 1.5f;
    public float duration = 0.2f;

    // Dictionnaire pour suivre l'état de sortie des objets
    private Dictionary<Collider, bool> objectExitStatus = new Dictionary<Collider, bool>();

    private void Start()
    {
        // Récupérer le MeshRenderer et le matériau original
        meshRenderer = GetComponent<MeshRenderer>();
        targetMaterial = meshRenderer.material;
        targetMaterial.SetFloat(shaderVariableName, 0.0f);
    }

    private void OnTriggerStay(Collider other)
    {
        // Vérifiez si l'objet est partiellement à l'extérieur
        bool isExiting = !IsObjectCompletelyInside(other);

        if (isExiting)
        {
            if (other.name == "LeftHand Controller" || other.name == "RightHand Controller")
            {
                if (!objectExitStatus.ContainsKey(other) || !objectExitStatus[other])
                {
                    Debug.Log("REMI : Stay -> 3 ");

                    // Si l'objet commence à sortir
                    StartCoroutine(AnimateShaderVariable(0.0f, targetValue));
                    objectExitStatus[other] = true;
                }
            }
        }
        else
        {
            if (objectExitStatus.ContainsKey(other) && objectExitStatus[other])
            {
                // Si l'objet cesse de sortir
                StartCoroutine(AnimateShaderVariable(targetValue, 0.0f));
                objectExitStatus[other] = false;
            }
        }
    }

    private bool IsObjectCompletelyInside(Collider other)
    {
        // Supposons que le collider du contrôleur soit une sphère
        SphereCollider sphereCollider = other as SphereCollider;
        if (sphereCollider == null) return true; // Si pas de SphereCollider, utilise la méthode des bounds

        Collider triggerCollider = GetComponent<Collider>();

        // Calculez la position du centre de la sphère en coordonnées mondiales
        Vector3 sphereCenter = other.transform.TransformPoint(sphereCollider.center);
        float sphereRadius = sphereCollider.radius * Mathf.Max(other.transform.lossyScale.x, other.transform.lossyScale.y, other.transform.lossyScale.z);

        // Vérifie si le centre de la sphère et les points à la surface sont à l'intérieur du trigger collider
        Vector3[] checkPoints = new Vector3[]
        {
            sphereCenter,
            sphereCenter + new Vector3(sphereRadius, 0, 0),
            sphereCenter + new Vector3(-sphereRadius, 0, 0),
            sphereCenter + new Vector3(0, sphereRadius, 0),
            sphereCenter + new Vector3(0, -sphereRadius, 0),
            sphereCenter + new Vector3(0, 0, sphereRadius),
            sphereCenter + new Vector3(0, 0, -sphereRadius)
        };

        foreach (Vector3 point in checkPoints)
        {
            Vector3 closestPoint = triggerCollider.ClosestPoint(point);
            if (closestPoint != point)
            {
                return false;
            }
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
            Debug.LogError("REMI : Material : Variable de shader introuvable ou le matériau est nul.");
        }
    }
}
