using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.userCapture;
using umi3d.common;
using umi3d.common.lbe;
using umi3d.common.lbe.description;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

namespace ClientLBE
{
    public class GuardianManager : SingleBehaviour<GuardianManager>
    {
        #region Fields

        [Header("CALIBRATION SCENE")]
        public GameObject Player;
        public Transform PersonnalSketletonContainer;
        public GameObject CameraPlayer;

        private Transform scene;

        [Header("GUARDIAN")]
        public Material GuardianMaterial;

        private GameObject guardianMesh;
        private GameObject guardianParent; 
        private List<Vector3> localVertexPositions = new List<Vector3>();
        private List<Quaternion> localVertexRotations = new List<Quaternion>();

        [Header("ANCHOR AR")]
        public ARAnchorManager AnchorManager; // Référence au gestionnaire d'ancres AR
        
        private List<Vector3> guardianAnchors = new List<Vector3>(); // Liste pour stocker toutes les ancres du guardian
        private UserGuardianDto userGuardianDto;
        private List<GameObject> tempVerticesTransform = new List<GameObject>();

        [Header("CALIBRATOR")]
        public GameObject ManualCalibrator;
        public Material OcclusionMaterial;
        public CanvasGroup OrientationScenePanel;

        private bool automaticCalibration = false;
        private Transform calibrator;
        private ARPlaneManager arPlaneManager;

        private float orientationOffset;

        private List<ARPlane> planesToCalibrate = new List<ARPlane>();
        private LBEGroupSyncRequestDTO lBEGroupDto = new LBEGroupSyncRequestDTO ();

        #endregion

        #region Methods

        public void Start()
        {
            //Desactivation du calibreur manuel au start
            if (automaticCalibration)
                ManualCalibrator.gameObject.SetActive(false);

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
                Debug.LogWarning("Bone not found.");

        }

        private void ARPlanesActivation(bool activation)
        {
            for (int i = 0; i < planesToCalibrate.Count; i++)
            {
                planesToCalibrate[i].gameObject.SetActive(activation);
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
                        planesToDestroy.Add(plane);

                    else
                        planesToCalibrate.Add(plane);
                }

                foreach (var plane in planesToDestroy)
                {
                    Destroy(plane.gameObject);
                }
            }
            else
                Debug.LogError("ARPlaneManager not found on this GameObject.");


            if (automaticCalibration)
            {
                if (planesToCalibrate.Count == 1)
                {
                    GameObject calibratorARPlane = new GameObject("Calibreur Plane");
                    calibratorARPlane.transform.position = planesToCalibrate[0].transform.position;
                    calibratorARPlane.transform.rotation = planesToCalibrate[0].transform.rotation;

                    calibrator = calibratorARPlane.transform;
                    calibrator.transform.parent = Player.transform;
                }

                else
                {
                    Debug.LogError("Multiple ARPlane detected. Only one ARPlane should be selected to serve as a calibrator. Change your environment configuration");
                    automaticCalibration = false;
                    SetManualCalibrator();
                }
            }

            else
                SetManualCalibrator();
        }

        public void ToggleCalibrationScene(bool value)
        {
            automaticCalibration = value;

            if (automaticCalibration)
                SetARPlaneCalibrator();

            else
                SetManualCalibrator();
        }

        private void SetARPlaneCalibrator()
        {
            ManualCalibrator.gameObject.SetActive(false);

            if (planesToCalibrate.Count > 0)
            {
                ARPlanesActivation(true);
                calibrator = planesToCalibrate[0].transform;
            }

            else
            {
                automaticCalibration = false;
                SetManualCalibrator();
            }
        }

        private void SetManualCalibrator()
        {
            ARPlanesActivation(false);

            ManualCalibrator.gameObject.SetActive(true);
            calibrator = ManualCalibrator.transform;

            if (OrientationScenePanel.gameObject.activeSelf == true && OrientationScenePanel.alpha == 1)
                OrientationScenePanel.GetComponent<GetPlayerOrientationPanel>().ClosePanel();
        }

        public void StartCalibrationScene()
        {
            StartCoroutine(CalibrationScene());
        }

        public void OrientationChoice(float orientation)
        {
            orientationOffset = orientation;
        }

        public void CloseOrientationChoice()
        {
            calibrator.transform.Rotate(calibrator.transform.rotation.x, orientationOffset, calibrator.transform.rotation.z, Space.World);

            OrientationScenePanel.gameObject.SetActive(false);
        }
        public IEnumerator CalibrationScene()
        {
            yield return null;
            yield return null;

            // TODO check the reason we have to wait

            if (automaticCalibration)
            {
                CloseOrientationChoice();
            }

            if (Player != null)
            {
                Transform parent = Player.transform.parent;

                if (parent != null)
                {
                    scene = parent;

                    calibrator.transform.rotation = new Quaternion(0.0f, calibrator.transform.rotation.y, 0.0f, calibrator.transform.rotation.w);
                    calibrator.transform.SetParent(null, true);
                    Player.transform.SetParent(calibrator.transform, true);

                    Vector3 Offset = Vector3.ProjectOnPlane(CameraPlayer.transform.position - calibrator.transform.position, Vector3.up);
                    calibrator.transform.Translate(Offset, Space.World);

                    float angle = Vector3.SignedAngle(CameraPlayer.transform.forward, Vector3.ProjectOnPlane(calibrator.transform.forward, Vector3.up), Vector3.up);
                    calibrator.transform.Rotate(0f, -angle, 0f);

                    Player.transform.SetParent(scene, true);
                    calibrator.transform.SetParent(Player.transform, true);

                    //Création du Parent des ancres
                    guardianParent = new GameObject("Guardian");
                    guardianParent.transform.position = new Vector3(calibrator.transform.position.x, 0.0f, calibrator.transform.position.z);

                    GetGuardianArea();
                }
                else
                    Debug.LogError(Player.name + " has no parents.");
            }
            else
                Debug.LogWarning("No GameObject to check is assigned!");

            arPlaneManager.enabled = false;
            ARPlanesActivation(false);
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

                    if (AnchorManager != null)
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
                    
                    guardianMesh = new GameObject("GuardianMesh");
                    guardianMesh.transform.position = Vector3.zero;
                    
                    CreateGuardianMesh(guardianAnchors);

                    if (AnchorManager.enabled == false)
                    {
                        AnchorManager.enabled = true;
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
            if (guardianMesh != null)
            {
                Destroy(guardianMesh);
                guardianMesh = null;
            }

            if (guardianAnchors.Count > 0)
            {
                guardianAnchors.Clear();
            }

            if (localVertexPositions.Count >0)
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

            guardianMesh = new GameObject("GuardianMesh");
            guardianMesh.transform.position = Vector3.zero;

            CreateGuardianMesh(VerticePos);

            guardianMesh.transform.position = new Vector3(calibrator.transform.position.x, 0.0f, calibrator.transform.position.z);
            guardianMesh.transform.rotation = calibrator.transform.rotation;
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
            guardianMesh.AddComponent<ARAnchor>();
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
                AnchorGuardianTemp.transform.parent = guardianMesh.transform;

                tempVerticesTransform.Add(AnchorGuardianTemp);
            }

            // Mesh Properties
            mesh.vertices = points.ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.Optimize();

            // Creation of the mesh renderer and filter
            MeshFilter meshFilter = guardianMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = guardianMesh.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;

            Material material = GuardianMaterial;
            meshRenderer.material = material;

            //Add box collider
            guardianMesh.AddComponent<BoxCollider>().isTrigger = true;

            guardianMesh.transform.position = Quaternion.Inverse(PersonnalSketletonContainer.transform.rotation) * (guardianMesh.transform.position) + PersonnalSketletonContainer.transform.position;
            guardianMesh.transform.rotation = PersonnalSketletonContainer.transform.rotation;

            guardianMesh.AddComponent<HoverGuardian>().targetMaterial = GuardianMaterial;

            guardianMesh.transform.parent = calibrator.transform;

            for (int i = 0; i < tempVerticesTransform.Count; i++)
            {
                tempVerticesTransform[i].transform.parent = null;
                tempVerticesTransform[i].transform.parent = calibrator.transform;

                localVertexPositions.Add(new Vector3 (tempVerticesTransform[i].transform.localPosition.x, tempVerticesTransform[i].transform.position.y, tempVerticesTransform[i].transform.localPosition.z));
                localVertexRotations.Add(tempVerticesTransform[i].transform.localRotation);
            }

            guardianMesh.transform.parent = null;
        }
    }
    #endregion
}