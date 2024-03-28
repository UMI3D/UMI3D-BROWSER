using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.Interaction.Toolkit;

using UnityEngine.XR.ARFoundation;
using TMPro;
using System.IO;
using UnityEngine.UI;
using static UnityEngine.XR.ARFoundation.ARAnchorSerializer;
using umi3d.common.lbe.description;
using umi3d.common;
using umi3d.cdk;

public class GuardianManager : MonoBehaviour
{
    private GameObject GuardianmeshObject;
    public string filePath;

    public GameObject pointAnchor; // Référence au prefab pour la création des ancres du guardian
    public Material Guardian; // Matériau pour le rendu du guardian

    private List<GameObject> guardianAnchors = new List<GameObject>(); // Liste pour stocker toutes les ancres du guardian

    public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
    //public ARAnchorSerializer anchorSerializer; // Référence à l'ARAnchorSerializer

    private UserGuardianDto userGuardianDto;




    private void Start()
    {
        GetGuardianArea();
    }

    public void DeleteGuardianJson()
    {
        /*foreach (var anchor in anchorManager.trackables)
        {
            SerializedARAnchor serializedAnchor = anchorSerializer.GetSerializedAnchor(anchor.trackableId.ToString());

            // Vérifiez si l'ancre est du guardian
            if (serializedAnchor != null)
            {
                // Supprimer l'ancre du guardian
                anchorManager.RemoveAnchor(anchor);
            }
        }*/

        if (guardianAnchors.Count > 0)
        {
            for (int i = 0; i < guardianAnchors.Count; i++)
            {

                Destroy(guardianAnchors[i]);
            }
            guardianAnchors.Clear();
        }


        if (GuardianmeshObject != null)
        {
            // Supprimer le mesh associé
            Destroy(GuardianmeshObject);
        }  
    }

    public void SendGuardianInServer(GameObject anchorGameObject)
    {
        // Créer une nouvelle instance de UserGuardianDto
        userGuardianDto = new UserGuardianDto();

        // Créer une nouvelle instance de ARAnchorDto pour stocker les données de l'ARAnchor
        ARAnchorDto arAnchorDto = new ARAnchorDto();

        // Obtenir le composant ARAnchor de l'ancrage
        ARAnchor arAnchor = anchorGameObject.GetComponent<ARAnchor>();

        // Ajouter les données de l'ARAnchorDto
        arAnchorDto.trackableId = arAnchor.trackableId.ToString();
        arAnchorDto.position = new Vector3Dto { X = arAnchor.transform.position.x, Y = arAnchor.transform.position.y, Z = arAnchor.transform.position.z };
        arAnchorDto.rotation = new Vector4Dto { X = arAnchor.transform.rotation.x, Y = arAnchor.transform.rotation.y, Z = arAnchor.transform.rotation.z, W = arAnchor.transform.rotation.w };

        // Ajouter l'ARAnchorDto à UserGuardianDto
        userGuardianDto.anchorAR.Add(arAnchorDto);

    }

    //A reprendre pour l'utilisation d'un seconde joueur pour réceptionner le guardian envoyer par le serveur
    public void LoadGuardianJson()
    {
        // Désérialiser les ancres à partir du fichier JSON
        //List<SerializedARAnchor> deserializedAnchors = anchorSerializer.DeserializeAnchorsFromJson(filePath);
        List<Vector3> anchorForMesh = new List<Vector3>();

        /*if (deserializedAnchors != null)
        {
            foreach (var serializedAnchor in deserializedAnchors)
            {
                // Convertir la pose sérialisée en Pose
                Vector3 position = new Vector3(serializedAnchor.pose.position.x, serializedAnchor.pose.position.y, serializedAnchor.pose.position.z);
                Quaternion rotation = new Quaternion(serializedAnchor.pose.rotation.x, serializedAnchor.pose.rotation.y, serializedAnchor.pose.rotation.z, serializedAnchor.pose.rotation.w);
                Pose pose = new Pose(position, rotation);


                anchorForMesh.Add(position);

                GameObject basePoint = Instantiate(pointAnchor, pose.position, pose.rotation);

                CreatGuardianLimitJson(basePoint);
            }
        }
        else
        {
            Debug.LogWarning("Aucune ancre n'a été désérialisée à partir du fichier JSON.");
        }*/

        //CreateMesh(anchorForMesh);
    }

    public void GetGuardianArea()
    {
        List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems<XRInputSubsystem>(inputSubsystems);

        if (inputSubsystems.Count > 0)
        {
            XRInputSubsystem inputSubsystem = inputSubsystems[0];
            if (!inputSubsystem.running)
            {
                inputSubsystem.Start();
            }
            List<Vector3> boundaryPoints = new List<Vector3>();

            if (inputSubsystem.TryGetBoundaryPoints(boundaryPoints))
            {
                if (anchorManager != null)
                {
                    foreach (Vector3 point in boundaryPoints)
                    {
                        // Instancier le prefab à la position du point récupéré                   
                        GameObject basePoint;
                        basePoint = Instantiate(pointAnchor, point, Quaternion.identity);

                        CreateGuardianLimit(basePoint);

                        // Envoyer la demande UMI3D contenant les données des ancres au serveur
                        UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);
                    }
                    // sérialisation des ancres pour sauvegarde dans le JSON
                    /*if (anchorSerializer != null)
                    {
                        List<ARAnchor> serialAnchors = new List<ARAnchor>();

                        foreach (GameObject anchor in guardianAnchors)
                        {
                            serialAnchors.Add(anchor.GetComponent<ARAnchor>());
                        }
                        anchorSerializer.SerializeAnchorsToJson(serialAnchors, filePath);
                    }
                    else
                    {
                        Debug.LogError("ARAnchorSerializer non défini.");
                    }*/
                }
                else
                {
                    Debug.LogError("AnchorManager non défini !");
                }

                List<Vector3> anchorForMesh = new List<Vector3>();

                foreach (GameObject anchor in guardianAnchors)
                {
                    anchorForMesh.Add(anchor.transform.position);
                }
                //CreateMesh(anchorForMesh);
            }
        }
        else
        {
            Debug.LogError("Aucun sous-système d'entrée XR disponible.");
        }
    }

    private void AddAnchor(GameObject basePoint)
    { 
        Vector3 basePointPosition = basePoint.transform.position;
        Quaternion basePointRotation = basePoint.transform.rotation;
        Pose basePointPose = new Pose(basePointPosition, basePointRotation);

        basePoint.AddComponent<ARAnchor>();

        guardianAnchors.Add(basePoint);
        anchorManager.AddAnchor(basePointPose);

        Transform BaseText = basePoint.transform.Find("Canvas/Panel/Text (TMP)");

        if (BaseText != null)
        {
            TextMeshProUGUI text = BaseText.GetComponent<TextMeshProUGUI>();

            if (text != null)
            {
                text.text = "X: " + basePoint.transform.position.x + ", Y: " + basePoint.transform.position.y + ", Z: " + basePoint.transform.position.z;
            }
        }
        else
        {
            Debug.LogWarning("Le GameObject \"Text (TMP)\" n'a pas été trouvé dans les enfants de l'objet : " + basePoint.name);
        }
    }

    //For SecondPlayer
    public void CreatGuardianLimitJson(GameObject basePoint)
    {
        AddAnchor(basePoint);
    }

    //For FirstPlayerManagezr
    public void CreateGuardianLimit(GameObject basePoint)
    {
        AddAnchor(basePoint);
        SendGuardianInServer(basePoint);

        GameObject basePointUp = Instantiate(pointAnchor, basePoint.transform.position, Quaternion.identity);
        basePointUp.transform.position = new Vector3(basePoint.transform.position.x, basePoint.transform.position.y + 2f, basePoint.transform.position.z);

        AddAnchor(basePointUp);
        SendGuardianInServer(basePointUp);
    }


    //A utiliser seulement sur le serveur pour ensuite l'envoyer à tous les clients
    private void CreateMesh(List<Vector3> points)
    {
         // Créer un nouveau GameObject pour le maillage
         GuardianmeshObject = new GameObject("GuardianMesh");
         GuardianmeshObject.transform.position = Vector3.zero;
         GuardianmeshObject.transform.rotation = Quaternion.identity;
         GuardianmeshObject.AddComponent<ARAnchor>();

         Mesh mesh = new Mesh();

         // Generer les point bassés sur les positions des ancres
         List<Vector3> bottomPoints = new List<Vector3>();
         List<Vector3> topPoints = new List<Vector3>();

        for (int i = 0; i < points.Count; i += 2)
        {
            bottomPoints.Add(points[i]); // Les points impairs sont les points bas
            topPoints.Add(points[i + 1]); // Les points pairs sont les points hauts
        }

        //Combine both bottom and top points
        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(bottomPoints);
        vertices.AddRange(topPoints);

        // Créationd des triangles
        int[] triangles = new int[]
         {
             // Sides
             0, 4, 1,
             1, 4, 5,
             1, 5, 2,
             2, 5, 6,
             2, 6, 3,
             3, 6, 7,
             3, 7, 0,
             0, 7, 4
         };

         // UVs
         Vector2[] uvs = new Vector2[vertices.Count];
         for (int i = 0; i < uvs.Length; i++)
         {
             uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
         }

         // propriété du mesh
         mesh.vertices = vertices.ToArray();
         mesh.triangles = triangles;
         mesh.uv = uvs;

         // création du mesh renderer et filter
         MeshFilter meshFilter = GuardianmeshObject.AddComponent<MeshFilter>();
         MeshRenderer meshRenderer = GuardianmeshObject.AddComponent<MeshRenderer>();

         // assigné le matérial
         Material material = Guardian;
         meshRenderer.material = material;
         meshFilter.mesh = mesh;
    }
}

