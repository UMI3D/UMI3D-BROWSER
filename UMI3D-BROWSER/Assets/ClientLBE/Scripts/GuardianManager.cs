using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.ARFoundation;
using TMPro;

using umi3d.common.lbe;
using umi3d.common.lbe.description;

using umi3d.common;
using umi3d.cdk;
using umi3d.common.collaboration.dto.signaling;
using umi3d.cdk.collaboration;

namespace ClientLBE
{
    public class GuardianManager : MonoBehaviour
    {
        private GameObject GuardianMesh; //Référence pour stocker le mesh du guardian

        public GameObject OriginGuardian;
        public GameObject GuardianParent;

        public GameObject pointAnchor; // Référence au prefab pour la création des ancres du guardian
        public Material MatGuardian; // Matériau pour le rendu du guardian

        private List<GameObject> guardianAnchors = new List<GameObject>(); // Liste pour stocker toutes les ancres du guardian
        private List<Transform> guardianAnchorsTransform = new List<Transform>(); // Liste pour stocker toutes les ancres du guardian

        public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
        
        private UserGuardianDto userGuardianDto;

        public GameObject Player;
        private GameObject Scene;
        public GameObject Calibreur;
        public Transform OriginGuardianServer;

        private Vector3 UserPosition;

        public GameObject RightHandController;
        public GameObject LeftHandController;

        public GameObject LeftHandTracking;
        public GameObject RightHandTrackingVariant;


        public void Start()
        {

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => CalibrationScene());
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => GetGuardianArea());

        }

        public void CalibrationScene()
        {

            if (Player != null)
            {
                // Obtenir le parent du GameObject Player (Scene)
                Transform parent = Player.transform.parent;

                if (parent != null)
                {
                    UserPosition = new Vector3(UMI3DEnvironmentClient.enterDto.userPosition.X, 0f, UMI3DEnvironmentClient.enterDto.userPosition.Z);
                    Debug.Log("Remi : UserPosition -> " + UMI3DEnvironmentClient.enterDto.userPosition.X + UMI3DEnvironmentClient.enterDto.userPosition.Y + UMI3DEnvironmentClient.enterDto.userPosition.Z);

                    //Création du Parent des ancres
                    GuardianParent = new GameObject("Guardian");
                    GuardianParent.transform.position = Vector3.zero;

                    Scene = parent.gameObject;

                    Player.transform.parent = null;

                    Scene.transform.position = new Vector3(Calibreur.transform.position.x, 0.0f, Calibreur.transform.position.z);
                    //Scene.transform.rotation = new Quaternion(Scene.transform.rotation.x, Calibreur.transform.rotation.y, Scene.transform.rotation.z, Scene.transform.rotation.w);

                    GameObject SceneRepere = Instantiate(pointAnchor, Scene.transform.position, Scene.transform.rotation);
                    Scene.transform.parent = Scene.transform;


                    Debug.Log("Remi : COORD -> " + UserPosition);
                    Debug.Log("Remi : Scene -> " + Scene.transform.position);

                    //Déplacement de la scène à l'origine du spawnpoint
                    Scene.transform.position = new Vector3(Scene.transform.position.x - UserPosition.x, 0.0f, Scene.transform.position.z - UserPosition.z);

                    Debug.Log("Remi : Scene -> " + Scene.transform.position);

                    Player.transform.parent = parent;

                    /* Calibreur.transform.position = new Vector3(Calibreur.transform.position.x + UserPosition.x, 0f, Calibreur.transform.position.y + UserPosition.y) ;
                     Player.transform.position = new Vector3(Player.transform.position.x + UserPosition.x, 0f, Player.transform.position.y + UserPosition.y);
                    */
                    Scene.transform.position = Vector3.zero;

                   

                }
                else
                {
                    Debug.Log("Remi : " + Player.name + " n'a pas de parent.");
                }
            }
            else
            {
                Debug.LogWarning("Remi : Aucun GameObject à vérifier n'est assigné !");
            }
        }

        void OnEnable()
        {
            UMI3DForgeClient.ImportantEventOccurred += HandleImportantEvent;
        }

        void OnDisable()
        {
            UMI3DForgeClient.ImportantEventOccurred -= HandleImportantEvent;
        }

        //Création du guardian à partir des données du serveur
        void HandleImportantEvent(UserGuardianDto userGuardianDto)
        {
            CreatGuardianServer(userGuardianDto);
        }


        public void GetGuardianArea()
        {
            if (userGuardianDto == null)
            {
                // Créer une nouvelle instance de UserGuardianDto
                userGuardianDto = new UserGuardianDto();
                userGuardianDto.anchorAR = new List<ARAnchorDto>();
            }

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

                //Récupération des 4 points de la play area fournis par le guardian du casque
                if (inputSubsystem.TryGetBoundaryPoints(boundaryPoints))
                {
                    Debug.Log("Remi : Start Get Boundary point");

                    if (anchorManager != null)
                    {
                        foreach (Vector3 point in boundaryPoints)
                        {
                            // Instancier le prefab à la position du point récupéré                   
                            GameObject AnchorGuardian = new GameObject("AnchorGuardianDown");
                            //AnchorGuardian = Instantiate(pointAnchor, GuardianParent.transform.position, Quaternion.identity);
                            AnchorGuardian.transform.position = point;
                            guardianAnchors.Add(AnchorGuardian);
                            // Instancier un second prefab à la position du point récupéré à Y+2                 

                            GameObject AnchorGuardianUp = new GameObject("AnchorGuardianUp");
                            //GameObject basePointUp = Instantiate(pointAnchor, AnchorGuardian.transform.position, Quaternion.identity);
                            AnchorGuardianUp.transform.position = new Vector3(AnchorGuardian.transform.position.x, AnchorGuardian.transform.position.y + 2f, AnchorGuardian.transform.position.z);

                            guardianAnchors.Add(AnchorGuardianUp);
                        }
                    }
                    else
                    {
                        Debug.LogError("Remi : AnchorManager non défini !");
                    }

                    //Mise en enfant des ancres sous le même GameObject GuardianParent
                    for (int i = 0; i < guardianAnchors.Count; i++)
                    {
                        guardianAnchors[i].transform.parent = GuardianParent.transform;
                    }

                    GuardianParent.transform.position = OriginGuardian.transform.position;
                    GuardianParent.transform.parent = OriginGuardian.transform;
                  
                    // ressortir les ancres rapatriés de du parent GuardianParent pour être enfant du Calibreur pour sauvegardé le bon transform
                    for (int i = 0; i < guardianAnchors.Count; i++)
                    {
                        guardianAnchors[i].transform.parent = null;
                        guardianAnchors[i].transform.parent = Calibreur.transform;

                        GameObject tempObject = new GameObject();
                        Transform tempTransform = tempObject.transform;

                        tempTransform.localPosition = guardianAnchors[i].transform.localPosition;
                        guardianAnchorsTransform.Add(tempTransform);

                        guardianAnchors[i].transform.parent = null;
                        guardianAnchors[i].transform.parent = GuardianParent.transform;

                        Destroy(tempObject);
                    }

                    if (anchorManager.enabled == false)
                    {
                        anchorManager.enabled = true;
                    }

                    //AddAnchorGuardian();
                    
                    SendGuardianInServer();

                    //Envoi des data du guardian au serveur
                    if (userGuardianDto != null)
                    {
                        Debug.Log("Remi : Count userGuardianDto -> " + userGuardianDto.anchorAR.Count);
                        StartCoroutine(WaitSendGuardian());
                    }
                    Debug.Log("Remi : End Guadrian Server");

                    List<Vector3> anchorForMesh = new List<Vector3>();

                    for(int i = 0; i < guardianAnchors.Count; i++)
                    {
                        anchorForMesh.Add(guardianAnchors[i].transform.position);
                    }
                    Debug.Log("Remi : End Add anchor");


                    CreateGuardianMesh(anchorForMesh);

                   

                    /*Debug.Log("Remi : Start Add anchor in Scene");
                    if (Scene != null)
                    {
                        Scene.AddComponent<ARAnchor>();
                    }
                    else
                    {
                        Debug.Log("Remi : Scene empty");
                    }
                    Debug.Log("Remi : End Add anchor in Scene");*/

                }
            }
            else
            {
                Debug.LogError("Remi : Aucun sous-système d'entrée XR disponible.");
            }
           // StartCoroutine(anchorScene());
        }

        IEnumerator anchorScene()
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Remi : Start Add anchor in Scene");
            if (Scene != null)
            {
                Scene.AddComponent<ARAnchor>();
            }
            else
            {
                Debug.Log("Remi : Scene empty");
            }
            Debug.Log("Remi : End Add anchor in Scene");
        }

        //Envoyer les data de chaque ancres au serveur
        public void SendGuardianInServer()
        {
            if (guardianAnchors != null || guardianAnchors.Count > 0)
            {

                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    /*ARAnchor arAnchor = guardianAnchors[i].GetComponent<ARAnchor>();
                    ARAnchorDto newAnchor = new ARAnchorDto();
                    string trackIdIn = arAnchor.trackableId.ToString();*/

                    ARAnchorDto newAnchor = new ARAnchorDto();
                    string trackIdIn = i.ToString();


                    if (ulong.TryParse(trackIdIn, out ulong trackIdOut))
                    {
                        newAnchor.trackableId = trackIdOut;
                    }

                    newAnchor.position = new Vector3Dto { X = guardianAnchorsTransform[i].transform.localPosition.x, Y = guardianAnchorsTransform[i].transform.localPosition.y, Z = guardianAnchorsTransform[i].transform.localPosition.z };
                    newAnchor.rotation = new Vector4Dto { X = guardianAnchorsTransform[i].transform.localRotation.x, Y = guardianAnchorsTransform[i].transform.localRotation.y, Z = guardianAnchorsTransform[i].transform.localRotation.z, W = guardianAnchorsTransform[i].transform.localRotation.w };

                    userGuardianDto.anchorAR.Add(newAnchor);
                    userGuardianDto.OffsetGuardian = Vector3.Distance(Calibreur.transform.position, OriginGuardian.transform.position);
                }

            }

        }

        IEnumerator WaitSendGuardian()
        {
            yield return new WaitForSeconds(2f);
            UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);
        }

        public void CreatGuardianServer(UserGuardianDto GuardianDto)
        {
            //Clean data first guardian in connection
            if (GuardianMesh != null)
            {
                Destroy(GuardianMesh);
            }

            if (guardianAnchors != null)
            {
                for(int i = 0; i<guardianAnchors.Count; i++)
                {
                    Destroy(guardianAnchors[i]);
                }
                guardianAnchors.Clear();
            }

            //Create new guardian server
            if (GuardianDto != null)
            {
                foreach (var point in GuardianDto.anchorAR)
                {
                    GameObject basePoint;
                    basePoint = Instantiate(pointAnchor, Calibreur.transform.position, Quaternion.identity);
                    basePoint.transform.parent = Calibreur.transform;
                    basePoint.transform.localPosition = Vector3.zero;
                    basePoint.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

                    basePoint.transform.localPosition = new Vector3(point.position.X, point.position.Y, point.position.Z);
                    basePoint.transform.localRotation = new Quaternion(point.rotation.X, point.rotation.Y, point.rotation.Z, point.rotation.W);

                    guardianAnchors.Add(basePoint);
                }

                List<Vector3> anchorForMesh = new List<Vector3>();

                foreach (GameObject anchor in guardianAnchors)
                {
                    anchorForMesh.Add(anchor.transform.position);
                }

                CreateGuardianMesh(anchorForMesh);

                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    guardianAnchors[i].transform.parent = null;
                    guardianAnchors[i].transform.parent = GuardianParent.transform;
                }

                //AddAnchorGuardian();
            }
        }

        public void AddAnchorGuardian()
        {
            //Définir les objets instancier représentant le guardian en ancres
            for (int i = 0; i < guardianAnchors.Count; i++)
            {
                Vector3 basePointPosition = guardianAnchors[i].transform.position;
                Quaternion basePointRotation = guardianAnchors[i].transform.rotation;
                Pose basePointPose = new Pose(basePointPosition, basePointRotation);

                guardianAnchors[i].AddComponent<ARAnchor>();
            }
        }

        //Création d'un mesh pour symbolier les limite du guardian à partir des ancres
        private void CreateGuardianMesh(List<Vector3> points)
        {
            // Créer un nouveau GameObject pour le maillage
            GuardianMesh = new GameObject("GuardianMesh");

            GuardianMesh.transform.parent = OriginGuardian.transform;
            GuardianMesh.transform.position = Vector3.zero;
            GuardianMesh.transform.rotation = Quaternion.identity;

            GuardianMesh.AddComponent<ARAnchor>();

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

            // Propriétés du mesh
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();


            // création du mesh renderer et filter
            MeshFilter meshFilter = GuardianMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = GuardianMesh.AddComponent<MeshRenderer>();

            // assigné le matérial
            Material material = MatGuardian;
            meshRenderer.material = material;
            meshFilter.mesh = mesh;

            // Ajout d'un MeshCollider
            MeshCollider meshCollider = GuardianMesh.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;  // Le mesh collider doit être convex pour être utilisé comme trigger
            meshCollider.isTrigger = true;

            GuardianMesh.AddComponent<HoverGuardian>();
        }
    }
}

