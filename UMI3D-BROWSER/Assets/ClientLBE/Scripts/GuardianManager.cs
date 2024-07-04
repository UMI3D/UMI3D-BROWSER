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
using BeardedManStudios.Forge.Networking.Unity;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.userCapture;
using umi3d.common.userCapture;

namespace ClientLBE
{
    public class GuardianManager : MonoBehaviour
    {
        private GameObject GuardianMesh; //Référence pour stocker le mesh du guardian

        public GameObject GuardianParent;

        public GameObject pointAnchor; // Référence au prefab pour la création des ancres du guardian
        public Material MatGuardian; // Matériau pour le rendu du guardian

        private List<Vector3> guardianAnchors = new List<Vector3>(); // Liste pour stocker toutes les ancres du guardian
        private List<Vector3> localVertexPositions = new List<Vector3>();
        private List<Quaternion> localVertexRotations = new List<Quaternion>();

        public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
        
        private UserGuardianDto userGuardianDto;

        public GameObject Player;
        private Transform Scene;
        public GameObject Calibreur;

        public Transform PersonnalSketletonContainer;
        public GameObject CameraPlayer;

        public GameObject XROrigine;

        private List<GameObject> TempVerticesTransform = new List<GameObject>();
        private ARPlaneManager arPlaneManager;

        public GameObject CollabSkeletonScene;
        public Material specificMaterial;


        public void Start()
        {
            arPlaneManager = XROrigine.GetComponent<ARPlaneManager>();
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => StartCalibrationScene());
            CollaborationSkeletonsManager.Instance.CollaborativeSkeletonCreated += OnCollaborativeSkeletonCreated;
        }

        public void Update()
        {
            Debug.Log("REMI : CollabSkeletonScene -> " + CollabSkeletonScene.transform.position);
        }

        void OnEnable()
        {
            UMI3DForgeClient.ImportantEventOccurred += HandleImportantEvent;

            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged += OnPlanesChanged;
            }
        }

        void OnDisable()
        {
            UMI3DForgeClient.ImportantEventOccurred -= HandleImportantEvent;

            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged -= OnPlanesChanged;
            }
        }

        void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            GetARPlanes();
        }

        private void OnCollaborativeSkeletonCreated(ulong userId)
        {

            // Récupérer le squelette de cet utilisateur
            var skeleton = CollaborationSkeletonsManager.Instance.GetCollaborativeSkeleton((UMI3DGlobalID.EnvironmentId, userId)) as AbstractSkeleton;

            if (skeleton != null)
            {
                // Ajouter une capsule
                AddCapsuleToBone(skeleton, BoneType.Hips);
            }
            else
            {
                Debug.LogWarning("Bone non trouvé pour cet utilisateur.");
            }
        }

        private void AddCapsuleToBone(AbstractSkeleton skeleton, uint boneType)
        {
            if (skeleton.Bones.TryGetValue(boneType, out var boneTransform))
            {
                // Créer une capsule
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

                capsule.transform.SetParent(skeleton.HipsAnchor);  

                capsule.transform.localPosition = new Vector3 (boneTransform.Position.x, 0f, boneTransform.Position.z);
                capsule.transform.localRotation = boneTransform.Rotation;
                capsule.transform.localScale = new Vector3(capsule.transform.localScale.x/1.25f, capsule.transform.localScale.y, capsule.transform.localScale.z/ 1.25f); // Ajustez si nécessaire

                Renderer capsuleRenderer = capsule.GetComponent<Renderer>();

                if (capsuleRenderer != null && specificMaterial != null)
                {
                    capsuleRenderer.material = specificMaterial;
                }
            }
            else
            {
                Debug.LogWarning("Bone non trouvé.");
            }
        }

        void GetARPlanes()
        {
            if (arPlaneManager != null)
            {
                List<ARPlane> planesToDestroy = new List<ARPlane>();

                foreach (ARPlane plane in arPlaneManager.trackables)
                {
                    if (plane.transform.position.y >= -0.2f && plane.transform.position.y <= 0.2f)
                    {
                        planesToDestroy.Add(plane);
                    }
                }

                foreach (ARPlane plane in planesToDestroy)
                {
                    Destroy(plane.gameObject);
                }
            }
            else
            {
                Debug.LogError("REMI : ARPlaneManager non trouvé sur ce GameObject.");
            }
        }

        public void StartCalibrationScene()
        {
            StartCoroutine(CalibrationScene());
        }
        
        public IEnumerator CalibrationScene()
        {
            yield return null;
            yield return null;

            if (Player != null)
            {
                Transform parent = Player.transform.parent;

                if (parent != null)
                {
                    Scene = parent;

                    Calibreur.transform.rotation = new Quaternion(0.0f, Calibreur.transform.rotation.y, 0.0f, Calibreur.transform.rotation.w);
                    Calibreur.transform.SetParent(null, true);
                    Player.transform.SetParent(Calibreur.transform, true);               

                    Vector3 Offset = Vector3.ProjectOnPlane(CameraPlayer.transform.position - Calibreur.transform.position, Vector3.up);
                    Calibreur.transform.Translate(Offset, Space.World);

                    float angle = Vector3.SignedAngle(CameraPlayer.transform.forward, Vector3.ProjectOnPlane(Calibreur.transform.forward, Vector3.up), Vector3.up);
                    Calibreur.transform.Rotate(0f, -angle, 0f);

                    Player.transform.SetParent(Scene, true);
                    Calibreur.transform.SetParent(Player.transform, true);

                    //Création du Parent des ancres
                    GuardianParent = new GameObject("Guardian");
                    GuardianParent.transform.position = new Vector3(Calibreur.transform.position.x, 0.0f, Calibreur.transform.position.z);

                    GetGuardianArea();
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
                    List<Vector3> MeshAnchor = new List<Vector3>();

                    if (anchorManager != null)
                    {
                        foreach (Vector3 point in boundaryPoints)
                        {
                            guardianAnchors.Add(point);
                            guardianAnchors.Add(point+ new Vector3(0f, 2f, 0f));
                        }
                    }
                    else
                    {
                        Debug.LogError("Remi : AnchorManager non défini !");
                    }
                    
                    GuardianMesh = new GameObject("GuardianMesh");
                    GuardianMesh.transform.position = Vector3.zero;
                    
                    CreateGuardianMesh(guardianAnchors);

                    if (anchorManager.enabled == false)
                    {
                        anchorManager.enabled = true;
                    }

                    AddAnchorGuardian();
                    
                    SendGuardianInServer();

                    //Envoi des data du guardian au serveur
                    if (userGuardianDto != null)
                    {
                        StartCoroutine(WaitSendGuardian());
                    }
                }
            }
            else
            {
                Debug.LogError("Remi : Aucun sous-système d'entrée XR disponible.");
            }
        }

        //Envoyer les data de chaque ancres au serveur
        public void SendGuardianInServer()
        {
            if (guardianAnchors != null || guardianAnchors.Count > 0)
            {
                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    ARAnchorDto newAnchor = new ARAnchorDto();
                    string trackIdIn = i.ToString();


                    if (ulong.TryParse(trackIdIn, out ulong trackIdOut))
                    {
                        newAnchor.trackableId = trackIdOut;
                    }

                    if(localVertexPositions.Count != 8)
                    {
                        Debug.LogError("Le nombre de Coordonnée n'est pas égal à 8");
                    }

                    newAnchor.position = new Vector3Dto { X = localVertexPositions[i].x, Y = localVertexPositions[i].y, Z = localVertexPositions[i].z };
                    newAnchor.rotation = new Vector4Dto { X = localVertexRotations[i].x, Y = localVertexRotations[i].y, Z = localVertexRotations[i].z, W = localVertexRotations[i].w };

                    userGuardianDto.anchorAR.Add(newAnchor);
                    userGuardianDto.OffsetGuardian = Vector3.Distance(Calibreur.transform.position, XROrigine.transform.position);
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
                GuardianMesh = null;
            }

            if (guardianAnchors.Count > 0)
            {
                guardianAnchors.Clear();
            }

            if(localVertexPositions.Count >0)
            {
                localVertexPositions.Clear();
            }

            if (localVertexRotations.Count > 0)
            {
                localVertexRotations.Clear();
            }

            List<Vector3> VerticePos = new List<Vector3>();
            List<Quaternion> VerticeRot = new List<Quaternion>();

            foreach (var point in GuardianDto.anchorAR)
            {
                VerticePos.Add(new Vector3(point.position.X, point.position.Y, point.position.Z));
                VerticeRot.Add(new Quaternion(point.rotation.X, point.rotation.Y, point.rotation.Z, point.rotation.W));
            }

            GuardianMesh = new GameObject("GuardianMesh");
            GuardianMesh.transform.position = Vector3.zero;

            CreateGuardianMesh(VerticePos);

            GuardianMesh.transform.position = new Vector3(Calibreur.transform.position.x, 0.0f, Calibreur.transform.position.z);
            GuardianMesh.transform.rotation = Calibreur.transform.rotation;
        }

        public void AddAnchorGuardian()
        {
            //Définir les objets instancier représentant le guardian en ancres
            for (int i = 0; i < guardianAnchors.Count; i++)
            {
                Vector3 basePointPosition = guardianAnchors[i];
                Quaternion basePointRotation = new Quaternion(0f,0f,0f,0f);
                Pose basePointPose = new Pose(basePointPosition, basePointRotation);

            }
            GuardianMesh.AddComponent<ARAnchor>();
        }

        private void CreateGuardianMesh(List<Vector3> points)
        {
            Mesh mesh = new Mesh();

            // Création des triangles
            int[] triangles = new int[]
            {
                // Face
                0, 1, 2,  2, 1, 3,
                4, 2, 3,  3, 5, 4,
                5, 7, 4,  4, 7, 6,
                6, 1, 0,  7, 1, 6,
            };

            // UVs
            Vector2[] uvs = new Vector2[points.Count];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(points[i].x, points[i].z);
            }

            for (int i = 0; i < points.Count; i++)
            {
                // Instancier le prefab à la position du point récupéré                   
                GameObject AnchorGuardianTemp = new GameObject("AnchorGuardianTemp");
                AnchorGuardianTemp.transform.position = points[i];
                AnchorGuardianTemp.transform.parent = GuardianMesh.transform;

                TempVerticesTransform.Add(AnchorGuardianTemp);
            }

            // Propriétés du mesh
            mesh.vertices = points.ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.Optimize();

            // création du mesh renderer et filter
            MeshFilter meshFilter = GuardianMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = GuardianMesh.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;

            // assigné le matérial
            Material material = MatGuardian;
            meshRenderer.material = material;

            //Ajout d'un box collider
            GuardianMesh.AddComponent<BoxCollider>().isTrigger = true;

            GuardianMesh.transform.position = Quaternion.Inverse(PersonnalSketletonContainer.transform.rotation) * (GuardianMesh.transform.position) + PersonnalSketletonContainer.transform.position;
            GuardianMesh.transform.rotation = PersonnalSketletonContainer.transform.rotation;

            GuardianMesh.AddComponent<HoverGuardian>().targetMaterial = MatGuardian;

            GuardianMesh.transform.parent = Calibreur.transform;

            for (int i = 0; i < TempVerticesTransform.Count; i++)
            {
                TempVerticesTransform[i].transform.parent = null;
                TempVerticesTransform[i].transform.parent = Calibreur.transform;

                localVertexPositions.Add(new Vector3 (TempVerticesTransform[i].transform.localPosition.x, TempVerticesTransform[i].transform.position.y, TempVerticesTransform[i].transform.localPosition.z));
                localVertexRotations.Add(TempVerticesTransform[i].transform.localRotation);
            }

            GuardianMesh.transform.parent = null;
        }
    }
}

