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
using System.Linq;

namespace ClientLBE
{
    public class GuardianManager : MonoBehaviour
    {
        #region Fields

        [Header("CALIBRATION SCENE")]
        public GameObject Player;
        public Transform PersonnalSketletonContainer;
        public GameObject CameraPlayer;

        private Transform Scene;

        [Header("GUARDIAN")]
        private GameObject GuardianMesh;
        private GameObject GuardianParent; 
        public Material MatGuardian;
        
        private List<Vector3> localVertexPositions = new List<Vector3>();
        private List<Quaternion> localVertexRotations = new List<Quaternion>();

        [Header("ANCHOR AR")]
        public ARAnchorManager anchorManager; // Référence au gestionnaire d'ancres AR
        private List<Vector3> guardianAnchors = new List<Vector3>(); // Liste pour stocker toutes les ancres du guardian
        private UserGuardianDto userGuardianDto;

        private List<GameObject> TempVerticesTransform = new List<GameObject>();

        [Header("CALIBREUR")]
        public bool AutomatiqueCalibration = false;
        public GameObject Calibreur;
        public GameObject ManualCalibreur;

        private ARPlaneManager arPlaneManager;
        public GameObject CollabSkeletonScene;
        public Material OcclusionMaterial;

        public CanvasGroup OrientationScenePanel;
        private float OrientationCalibreur;

        private List<ARPlane> planesToCalibrate = new List<ARPlane>();
        private LBEGroupSyncRequestDTO lBEGroupDto = new LBEGroupSyncRequestDTO ();

        #endregion

        #region Methods

        public void Start()
        {
            //Desactivation du calibreur manuel au start
            if (AutomatiqueCalibration == true)
            {
                Calibreur.SetActive(false);
                Calibreur = null;
            }

            arPlaneManager = this.GetComponent<ARPlaneManager>();
            StartCoroutine(GetARPlanes());

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => StartCalibrationScene());
        }

        void OnEnable()
        {
            UMI3DForgeClient.LBEGroupEventOccurred += LBEGroupEvent;
            UMI3DForgeClient.AddLBEGroupEvent += AddUserLBEGroup;
            UMI3DForgeClient.DelLBEGroupEvent += DelUserLBEGroup;

            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged += OnPlanesChanged;
            }
        }

        void OnDisable()
        {
            UMI3DForgeClient.LBEGroupEventOccurred -= LBEGroupEvent;
            UMI3DForgeClient.AddLBEGroupEvent -= AddUserLBEGroup;
            UMI3DForgeClient.DelLBEGroupEvent -= DelUserLBEGroup;

            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged -= OnPlanesChanged;
            }
        }

        void LBEGroupEvent(LBEGroupSyncRequestDTO LbeGroupDtoData)
        {
            lBEGroupDto = LbeGroupDtoData;
            CreatGuardianServer(lBEGroupDto.ARAnchors);
            AddCapsulesToCurrentARUsers();   
        }

        void AddUserLBEGroup(AddUserGroupOperationsDto addUserLBEGroupDTO)
        {
            if (addUserLBEGroupDTO.IsUserAR == true)
            {
                lBEGroupDto.UserAR.Add(addUserLBEGroupDTO.UserId);
                AddCapsulesToCurrentARUsers();
            }
            else
            {
                lBEGroupDto.UserVR.Add(addUserLBEGroupDTO.UserId);
            }
        }

        void DelUserLBEGroup(DelUserGroupOperationsDto delUserLBEGroupDto)
        {
            foreach (ulong userIdAR in lBEGroupDto.UserAR)
            {
                if(userIdAR == delUserLBEGroupDto.UserId)
                {
                    lBEGroupDto.UserAR.Remove(delUserLBEGroupDto.UserId);
                    return;
                }
                else
                {
                    Debug.Log("Not AR user");
                }
            }
            foreach (ulong userIdVR in lBEGroupDto.UserVR)
            {
                if (userIdVR == delUserLBEGroupDto.UserId)
                {
                    lBEGroupDto.UserVR.Remove(delUserLBEGroupDto.UserId);
                    return;
                }
                else
                {
                    Debug.Log("Not VR user");
                }
            }
        }

        void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            StartCoroutine(GetARPlanes());
        }

        private void AddCapsulesToCurrentARUsers()
        {
            foreach (var userId in lBEGroupDto.UserAR)
            {
                var skeleton = CollaborationSkeletonsManager.Instance.GetCollaborativeSkeleton((UMI3DGlobalID.EnvironmentId, userId)) as AbstractSkeleton;

                if (skeleton != null)
                {
                    AddCapsuleToBone(skeleton, BoneType.Hips);
                }
                else
                {
                    Debug.LogWarning($"user AR with ID : {userId} not found in scene.");
                }
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

                capsule.transform.localPosition = new Vector3(0f, 0f, 0f);

                capsule.transform.localRotation = boneTransform.Rotation;
                capsule.transform.localScale = new Vector3(capsule.transform.localScale.x/1.25f, capsule.transform.localScale.y, capsule.transform.localScale.z/ 1.25f); // Ajustez si nécessaire

                Renderer capsuleRenderer = capsule.GetComponent<Renderer>();

                if (capsuleRenderer != null && OcclusionMaterial != null)
                {
                    capsuleRenderer.material = OcclusionMaterial;
                }
            }
            else
            {
                Debug.LogWarning("Bone not found.");
            }
        }

        public IEnumerator GetARPlanes()
        {
            //Délais d'attente pour que arPlaneManager.trackables retourne des ARplanes
            yield return new WaitForSeconds(0.7f);

            List<ARPlane> planesToDestroy = new List<ARPlane>();

            if (arPlaneManager != null)
            {
                var trackables = arPlaneManager.trackables;

                foreach (var plane in trackables)
                {

                    if (plane.transform.position.y >= -0.5f && plane.transform.position.y <= 0.5f || plane.transform.position.y > 1.2f)
                    {
                        planesToDestroy.Add(plane);
                    }
                    else
                    {
                        planesToCalibrate.Add(plane);
                    }
                }

                foreach (var plane in planesToDestroy)
                {
                    Destroy(plane.gameObject);
                }
            }
            else
            {
                Debug.LogError("ARPlaneManager not found on this GameObject.");
            }

            if(AutomatiqueCalibration == true)
            {
                if (planesToCalibrate.Count == 1)
                {
                    GameObject CalibreurARPlane = new GameObject("Calibreur Plane");
                    CalibreurARPlane.transform.position = planesToCalibrate[0].transform.position;
                    CalibreurARPlane.transform.rotation = planesToCalibrate[0].transform.rotation;

                    Calibreur = CalibreurARPlane;
                    Calibreur.transform.parent = Player.transform;
                }

                else
                {
                    Debug.LogError("Multiple ARPlane detected. Only one ARPlane should be selected to serve as a calibrator. Change your environment configuration");
                }
            }    
        }

        public void ToggleCalibrationScene(bool arg)
        {
            AutomatiqueCalibration = arg;

            if (arg)
            {
                Calibreur.SetActive(false);

                if (planesToCalibrate.Count > 0)
                {
                    for (int i = 0; i < planesToCalibrate.Count; i++)
                    {
                        planesToCalibrate[i].gameObject.SetActive(true);
                    }
                    Calibreur = planesToCalibrate[0].gameObject;
                }
            }

            else
            {
                Calibreur = null;

                for (int i = 0; i < planesToCalibrate.Count; i++)
                {
                    planesToCalibrate[i].gameObject.SetActive(false);
                }

                Calibreur = ManualCalibreur ;
                Calibreur.SetActive(true);

                if (OrientationScenePanel.gameObject.activeSelf == true && OrientationScenePanel.alpha == 1)
                {
                    OrientationScenePanel.GetComponent<GetPlayerOrientationPanel>().ClosePanel();
                }
            }
        }

        public void StartCalibrationScene()
        {
            StartCoroutine(CalibrationScene());
        }

        public void OrientationChoice(float Orientaion)
        {
            OrientationCalibreur = Orientaion;
        }

        public void CloseOrientaionChoice()
        {
            Calibreur.transform.Rotate(Calibreur.transform.rotation.x, OrientationCalibreur, Calibreur.transform.rotation.z, Space.World);

            OrientationScenePanel.gameObject.SetActive(false);
        }
        public IEnumerator CalibrationScene()
        {
            yield return null;
            yield return null;

            if(AutomatiqueCalibration == true)
            {
                CloseOrientaionChoice();
            }

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
                    Debug.LogError(Player.name + " has no parents.");
                }
            }
            else
            {
                Debug.LogWarning("No GameObject to check is assigned!");
            }

            arPlaneManager.enabled = false;

            // Désactiver chaque plan
            for (int i = 0; i<planesToCalibrate.Count; i++)
            {
                planesToCalibrate[i].gameObject.SetActive(false);
            }
        }        

        public void GetGuardianArea()
        {
            if (userGuardianDto == null)
            {
                // Créer une nouvelle instance de UserGuardianDto
                userGuardianDto = new UserGuardianDto();
                userGuardianDto.ARAnchors = new List<ARAnchorDto>();
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
                        Debug.LogError("AnchorManager not referenced !");
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

                    // Sending data from the guardian to the server
                    if (userGuardianDto != null)
                    {
                        StartCoroutine(WaitSendGuardian());
                    }
                }
            }
            else
            {
                Debug.LogError("No XR input subsystem available.");
            }
        }

        // Envoyer les data de chaque ancres au serveur
        public void SendGuardianInServer()
        {
            if (guardianAnchors != null || guardianAnchors.Count > 0)
            {
                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    ARAnchorDto newAnchor = new ARAnchorDto();

                    if (localVertexPositions.Count != 8)
                    {
                        Debug.LogError("ARAnchor coordinate number is not equal to 8");
                    }

                    newAnchor.position = new Vector3Dto { X = localVertexPositions[i].x, Y = localVertexPositions[i].y, Z = localVertexPositions[i].z };
                    newAnchor.rotation = new Vector4Dto { X = localVertexRotations[i].x, Y = localVertexRotations[i].y, Z = localVertexRotations[i].z, W = localVertexRotations[i].w };
               
                    userGuardianDto.ARAnchors.Add(newAnchor);              
                }

                var loadingParameters = UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters;
                userGuardianDto.ARiD = loadingParameters.BrowserType;
            }
        }

        IEnumerator WaitSendGuardian()
        {
            yield return new WaitForSeconds(2f);
            UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);
        }

        public void CreatGuardianServer(List<ARAnchorDto> GuardianDto)
        {
            // Clear the client's first connection data
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

            //Retrieval of data sent by the server for creation of the guardian
            List<Vector3> VerticePos = new List<Vector3>();
            List<Quaternion> VerticeRot = new List<Quaternion>();

            foreach (var point in GuardianDto)
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
            //Define instantiated objects representing the guardian as ARAnchors
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

            // Creating triangles
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
                // Instantiate the prefab at the position of the retrieved point                  
                GameObject AnchorGuardianTemp = new GameObject("AnchorGuardianTemp");
                AnchorGuardianTemp.transform.position = points[i];
                AnchorGuardianTemp.transform.parent = GuardianMesh.transform;

                TempVerticesTransform.Add(AnchorGuardianTemp);
            }

            // Mesh Properties
            mesh.vertices = points.ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.Optimize();

            // Creation of the mesh renderer and filter
            MeshFilter meshFilter = GuardianMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = GuardianMesh.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;

            Material material = MatGuardian;
            meshRenderer.material = material;

            //Add box collider
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
    #endregion
}

