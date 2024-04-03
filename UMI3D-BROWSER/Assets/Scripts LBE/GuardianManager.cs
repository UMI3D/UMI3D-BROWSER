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
using umi3d.common.lbe;
using umi3d.common;
using umi3d.cdk;
using umi3d.common.collaboration.dto.signaling;
using System;

public class GuardianManager : MonoBehaviour
{
    private GameObject GuardianmeshObject;

    //public string filePath;

    public GameObject OriginGuardian;
    public GameObject GuardianParent;  

    public GameObject pointAnchor; // Référence au prefab pour la création des ancres du guardian
    public Material Guardian; // Matériau pour le rendu du guardian

    private List<GameObject> guardianAnchors = new List<GameObject>(); // Liste pour stocker toutes les ancres du guardian

    public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
    //public ARAnchorSerializer anchorSerializer; // Référence à l'ARAnchorSerializer

    private UserGuardianDto userGuardianDto;
    private JoinDto joinDto;



    public void Start()
    {
        Debug.Log("Remi : START Getguardian");
        UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(()=>GetGuardianArea());
        //GetGuardianArea();
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
        Debug.Log("Remi : Stat send serveur");
        ARAnchor arAnchor = anchorGameObject.GetComponent<ARAnchor>();

        string trackIdIn = arAnchor.trackableId.ToString();

        if (ulong.TryParse(trackIdIn, out ulong trackIdOut))
        {
            userGuardianDto.trackableId = trackIdOut;
        }
        
        userGuardianDto.position = new Vector3Dto { X = arAnchor.transform.position.x, Y = arAnchor.transform.position.y, Z = arAnchor.transform.position.z };
        userGuardianDto.rotation = new Vector4Dto { X = arAnchor.transform.rotation.x, Y = arAnchor.transform.rotation.y, Z = arAnchor.transform.rotation.z, W = arAnchor.transform.rotation.w };

        Debug.Log("Remi : " + userGuardianDto.trackableId);
        Debug.Log("Remi : " + userGuardianDto.position);
        Debug.Log("Remi : " + userGuardianDto.rotation);

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

        GuardianParent = new GameObject("Guardian");
        GuardianParent.transform.position = Vector3.zero; 

        Debug.Log("Remi : Getguardian");
        List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems<XRInputSubsystem>(inputSubsystems);

        if (inputSubsystems.Count > 0)
        {
            Debug.Log("Remi : Input System");

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
                        basePoint = Instantiate(pointAnchor, GuardianParent.transform.position, Quaternion.identity);
                        basePoint.transform.position = point;

                        // Créer une nouvelle instance de UserGuardianDto
                        userGuardianDto = new UserGuardianDto();

                        CreateGuardianLimit(basePoint);                  
                    }                 
                }
                else
                {
                    Debug.LogError("Remi : AnchorManager non défini !");
                }

                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    guardianAnchors[i].transform.parent = GuardianParent.transform;
                }

                GuardianParent.transform.position = OriginGuardian.transform.position;
                GuardianParent.transform.parent = OriginGuardian.transform;

                List<Vector3> anchorForMesh = new List<Vector3>();

                foreach (GameObject anchor in guardianAnchors)
                {
                    anchorForMesh.Add(anchor.transform.position);
                }
                CreateMesh(anchorForMesh);


                if(userGuardianDto != null)
                {
                    Debug.Log("Remi : not null");
                    UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);
                }
                
                // Envoyer la demande UMI3D contenant les données des ancres au serveur
                //UMI3DClientServer.SendRequest(joinDto, reliable: true);

            }
        }
        else
        {
            Debug.LogError("Remi : Aucun sous-système d'entrée XR disponible.");
        }

        Debug.Log("Remi : END get guardian");
    }

    public void AddAnchorGuardian(GameObject basePoint)
    {
        Debug.Log("Remi : Stat add anchor");

        if(anchorManager.enabled == false)
        {
            Debug.Log("Remi : Stat anchor manager");

            anchorManager.enabled = true;
            Debug.Log("Remi : OK anchor manager");

        }

        Vector3 basePointPosition = basePoint.transform.position;
        Quaternion basePointRotation = basePoint.transform.rotation;
        Pose basePointPose = new Pose(basePointPosition, basePointRotation);

        basePoint.AddComponent<ARAnchor>();

        guardianAnchors.Add(basePoint);

        Debug.Log("Remi : Before add anchor");

        //anchorManager.AddAnchor(basePointPose);

        Debug.Log("Remi : After add anchor");


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
        Debug.Log("Remi : End add anchor");

    }


    //For SecondPlayer
    public void CreatGuardianLimitJson(GameObject basePoint)
    {
        AddAnchorGuardian(basePoint);
    }

    //For FirstPlayerManagezr
    public void CreateGuardianLimit(GameObject basePoint)
    {
        Debug.Log("Remi : Start Creata guardian");

        AddAnchorGuardian(basePoint);
        SendGuardianInServer(basePoint);

        GameObject basePointUp = Instantiate(pointAnchor, basePoint.transform.position, Quaternion.identity);
        basePointUp.transform.position = new Vector3(basePoint.transform.position.x, basePoint.transform.position.y + 2f, basePoint.transform.position.z);

        AddAnchorGuardian(basePointUp);
        SendGuardianInServer(basePointUp);
        Debug.Log("Remi : End Creata guardian");

    }


    //A utiliser seulement sur le serveur pour ensuite l'envoyer à tous les clients
    private void CreateMesh(List<Vector3> points)
    {
        Debug.Log("Remi : Start Creat Mesh");

        // Créer un nouveau GameObject pour le maillage
         GuardianmeshObject = new GameObject("GuardianMesh");

        GuardianmeshObject.transform.parent = OriginGuardian.transform;
         GuardianmeshObject.transform.position = Vector3.zero;
         GuardianmeshObject.transform.rotation = Quaternion.identity;

        LogObjectHierarchy(GuardianmeshObject.transform);
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

        Debug.Log("Remi : End Creat Mesh");

    }

    void LogObjectHierarchy(Transform objTransform)
    {
        // Variable pour stocker la hiérarchie de l'objet
        string hierarchy = objTransform.name;

        // Parcours de la hiérarchie parentale de l'objet
        while (objTransform.parent != null)
        {
            hierarchy = objTransform.parent.name + "/" + hierarchy;
            objTransform = objTransform.parent;
        }

        // Affichage de la hiérarchie dans la console Unity
        Debug.Log("Remi : Hiérarchie de l'objet " + GuardianmeshObject.name + ": " + hierarchy);
    }
}

