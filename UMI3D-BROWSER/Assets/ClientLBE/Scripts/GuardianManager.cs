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
        private GameObject GuardianmeshObject;

        //public string filePath;

        public GameObject OriginGuardian;
        public GameObject GuardianParent;

        public GameObject pointAnchor; // Référence au prefab pour la création des ancres du guardian
        public Material Guardian; // Matériau pour le rendu du guardian

        private List<GameObject> guardianAnchors = new List<GameObject>(); // Liste pour stocker toutes les ancres du guardian
        private List<ARAnchor> aRAnchors = new List<ARAnchor>(); // Liste pour stocker toutes les ancres du guardian

        public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
                                              //public ARAnchorSerializer anchorSerializer; // Référence à l'ARAnchorSerializer

        public ARPointCloudManager pointCloudManager;

        
        private UserGuardianDto userGuardianDto;
        private ARAnchorDto anchorAR;
        private JoinDto joinDto;




        public void Start()
        {
            Debug.Log("Remi : START Getguardian");

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => GetGuardianArea());


            // Evénement pour écouter les mises à jour du nuage de points
            pointCloudManager.pointCloudsChanged += OnPointCloudsChanged;

        }

        void OnPointCloudsChanged(ARPointCloudChangedEventArgs obj)
        {

            Debug.Log("Remi : POINT obj -> " + obj);

            List<Vector3> addedpoints = new List<Vector3>();
            foreach(ARPointCloud pointCloud in obj.added)
            {
                Debug.Log("Remi : POINT pointCloud -> " + pointCloud);

                foreach (Vector3 pos in pointCloud.positions)
                {
                    Debug.Log("Remi : POINT CLOUD -> " + pos);
                    addedpoints.Add(pos);
                }
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

        void HandleImportantEvent(UserGuardianDto userGuardianDto)
        {
            Debug.Log("Remi : An important event occurred!");
            CreatGuardianServer(userGuardianDto);
        }

        public void SendGuardianInServer(GameObject anchorGameObject)
        {
            Debug.Log("Remi : Before SendGuardianInServer -> " + userGuardianDto.anchorAR.Count);
            ARAnchor arAnchor = anchorGameObject.GetComponent<ARAnchor>();
            ARAnchorDto newAnchor = new ARAnchorDto();

            string trackIdIn = arAnchor.trackableId.ToString();

            if (ulong.TryParse(trackIdIn, out ulong trackIdOut))
            {
                newAnchor.trackableId = trackIdOut;
            }

            newAnchor.position = new Vector3Dto { X = arAnchor.transform.position.x, Y = arAnchor.transform.position.y, Z = arAnchor.transform.position.z };
            newAnchor.rotation = new Vector4Dto { X = arAnchor.transform.rotation.x, Y = arAnchor.transform.rotation.y, Z = arAnchor.transform.rotation.z, W = arAnchor.transform.rotation.w };

            Debug.Log("Remi : newAnchor -> " + newAnchor);


            userGuardianDto.anchorAR.Add(newAnchor);

            Debug.Log("Remi : After SendGuardianInServer -> " + userGuardianDto.anchorAR.Count);

        }

        public void GetGuardianArea()
        {
            GuardianParent = new GameObject("Guardian");
            GuardianParent.transform.position = Vector3.zero;

            if (userGuardianDto == null)
            {
                Debug.Log("Remi : Create new UserGuardianDto");
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


                    if (userGuardianDto != null)
                    {
                        Debug.Log("Remi : Count userGuardianDto -> " + userGuardianDto.anchorAR.Count);
                        StartCoroutine(WaitSendGuardian());
                    }
                }
            }
            else
            {
                Debug.LogError("Remi : Aucun sous-système d'entrée XR disponible.");
            }
        }

        IEnumerator WaitSendGuardian()
        {
            yield return new WaitForSeconds(2f);
            UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);

        }

        public void CreatGuardianServer(UserGuardianDto GuardianDto)
        {
            //Clean data guardian connection
            Debug.Log("Remi : CreatGuardianServer");
            if (GuardianmeshObject != null)
            {
                Debug.Log("Remi : Destroy first local guardian");
                Destroy(GuardianmeshObject);
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
                    basePoint = Instantiate(pointAnchor, GuardianParent.transform.position, Quaternion.identity);

                    basePoint.transform.position = new Vector3(point.position.X, point.position.Y, point.position.Z);
                    basePoint.transform.rotation = new Quaternion(point.rotation.X, point.rotation.Y, point.rotation.Z, point.rotation.W);

                    CreateGuardianLimitServer(basePoint);
                }

               /* for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    guardianAnchors[i].transform.parent = GuardianParent.transform;
                }

                GuardianParent.transform.position = OriginGuardian.transform.position;
                GuardianParent.transform.parent = OriginGuardian.transform;*/

                List<Vector3> anchorForMesh = new List<Vector3>();

                foreach (GameObject anchor in guardianAnchors)
                {
                    anchorForMesh.Add(anchor.transform.position);
                }
                CreateMesh(anchorForMesh);
            }
        }

        public void CreateGuardianLimitServer(GameObject basePoint)
        {
            AddAnchorGuardian(basePoint);
        }

        public void CreateGuardianLimit(GameObject basePoint)
        {
            AddAnchorGuardian(basePoint);
            SendGuardianInServer(basePoint);

            GameObject basePointUp = Instantiate(pointAnchor, basePoint.transform.position, Quaternion.identity);
            basePointUp.transform.position = new Vector3(basePoint.transform.position.x, basePoint.transform.position.y + 2f, basePoint.transform.position.z);

            AddAnchorGuardian(basePointUp);
            SendGuardianInServer(basePointUp);
        }

        public void AddAnchorGuardian(GameObject basePoint)
        {
            if (anchorManager.enabled == false)
            {
                anchorManager.enabled = true;
            }

            Vector3 basePointPosition = basePoint.transform.position;
            Quaternion basePointRotation = basePoint.transform.rotation;
            Pose basePointPose = new Pose(basePointPosition, basePointRotation);

            basePoint.AddComponent<ARAnchor>();

            guardianAnchors.Add(basePoint);

            /* var Anchors = anchorManager.AddAnchor(basePointPose);

            if(Anchors == null)
            {
                Debug.Log("Remi : Echec de la création de l'ancre");
            }*/

            //Ajouter les coordonées sur le panel de l'ancre
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



        private void CreateMesh(List<Vector3> points)
        {
            Debug.Log("Remi : Start Creat Mesh");

            // Créer un nouveau GameObject pour le maillage
            GuardianmeshObject = new GameObject("GuardianMesh");

            GuardianmeshObject.transform.parent = OriginGuardian.transform;
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

            Debug.Log("Remi : End Creat Mesh");
        }
    }
}

