/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using inetum.unityUtils;
using inetum.unityUtils.observation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using umi3d.browserRuntime.NotificationKeys;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.userCapture;
using umi3d.common;
using umi3d.common.core;
using umi3d.common.lbe;
using umi3d.common.lbe.description;
using umi3d.common.userCapture;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace umi3d.VRBase.lbe
{
    public class GuardianManager : SingleBehaviour<GuardianManager>
    {
        #region Fields

        [Header("CALIBRATION SCENE")]
        public GameObject Player;
        public Transform PersonnalSkeletonContainer;
        public GameObject CameraPlayer;

        public GameObject Repere;
        public GameObject XROrigin;

        [Header("GUARDIAN")]
        public Material GuardianMaterial;

        private GameObject guardianMesh;
        private GameObject guardianParent;
        private List<Vector3> localVertexPositions = new List<Vector3>();
        private List<Quaternion> localVertexRotations = new List<Quaternion>();

        [Header("ANCHOR AR")]
        public Transform AnchorManager; // Référence au gestionnaire d'ancres AR

        private List<Vector3> guardianAnchors = new List<Vector3>(); // Liste pour stocker toutes les ancres du guardian
        //private UserGuardianDto userGuardianDto = new UserGuardianDto();
        private List<GameObject> tempVerticesTransform = new List<GameObject>();

        [Header("CALIBRATOR")]
        public GameObject ManualCalibrator;
        public Material OcclusionMaterial;
        public CanvasGroup OrientationScenePanel;

        //public bool automaticCalibration = true;
        public Transform calibrator;
        public ARPlaneManager arPlaneManager;

        public ARPlaneType ARPlaneFromType = ARPlaneType.Table;
        public ARPlaneType ARPlaneToType = ARPlaneType.Window;

        private ARPlane ARPlaneFrom;
        private ARPlane ARPlaneTo;

        private float orientationOffset;

        private List<ARPlane> planesToCalibrate = new List<ARPlane>();

        //List<ulong> colocatedUserIds = new List<ulong>();

        private Dictionary<string, System.Object> info = new();
        static bool isLeader = false;

        public event Action<Vector3> OnPositionCalibratorStart;

        public enum ARPlaneType
        {
            Table,
            Seat,
            Wall,
            Window,
            Desk,
            Floor,
            Ceiling,
            Other
        }
        #endregion

        #region Method

        public void Start()
        {
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsColocatedDevice = false;
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).LBEGroupId = 0;
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsLBEGroupLeader = false;
            
            //Desactivation du calibreur manuel au start
            //if (automaticCalibration)
                //ManualCalibrator.gameObject.SetActive(false);

            //arPlaneManager = this.GetComponent<ARPlaneManager>();

            //UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => StartCalibrationScene());
        }


#if UMI3D_XR
        void OnEnable()
        {
            //UMI3DForgeClient.LBEGroupSyncEvent += OnLBEGroupSync;
            UMI3DForgeClient.LBEUserAddedEvent += OnLBEUserAdded;
            UMI3DForgeClient.LBEUserRemovedEvent += OnLBEUserRemoved;
            UMI3DForgeClient.LBEActivationEvent += OnLBEActivation;
            UMI3DForgeClient.LBEGroupEvent += OnLBESetGroupReception;
            UMI3DForgeClient.LBELeaderEvent += OnLBELeaderReception;
            UMI3DForgeClient.LBEGuardianEvent += OnLBEGuardianReception;


            //if (arPlaneManager != null)
            //{
            //    arPlaneManager.planesChanged += OnPlanesChanged;
            //}
        }

        void OnDisable()
        {
            //UMI3DForgeClient.LBEGroupSyncEvent -= OnLBEGroupSync;
            UMI3DForgeClient.LBEUserAddedEvent -= OnLBEUserAdded;
            UMI3DForgeClient.LBEUserRemovedEvent -= OnLBEUserRemoved;
            UMI3DForgeClient.LBEActivationEvent -= OnLBEActivation;
            UMI3DForgeClient.LBEGroupEvent -= OnLBESetGroupReception;
            UMI3DForgeClient.LBELeaderEvent -= OnLBELeaderReception;
            UMI3DForgeClient.LBEGuardianEvent -= OnLBEGuardianReception;

            //if (arPlaneManager != null)
            //{
            //    arPlaneManager.planesChanged -= OnPlanesChanged;
            //}
        }
#endif

        public static bool isLBELeader()
        {
            return isLeader;
        }

        //void OnLBEGroupSync(LBEGroupSyncRequestDto dto)
        //{
        //    if (lBEGroupDto == null)
        //    {
        //        Debug.Log("REMY -> lBEGroupDto = null");
        //        return;
        //    }
        //    if (lBEGroupDto.UserAR.Count + lBEGroupDto.UserVR.Count > 0)
        //    {
        //        CreateGuardianServer(lBEGroupDto.ARAnchors);
        //        AddCapsulesToColocatedUsers();
        //    }
        //}

        void OnLBEUserAdded(LBEAddUserGroupOperationDto dto)
        {
            //colocatedUserIds.Add(dto.userId);
            OcclusionForColocatedUsers(new List<ulong>() { dto.userId });
        }

        void OnLBEUserRemoved(LBERemoveUserGroupOperationDto dto)
        {
            //colocatedUserIds.Remove(dto.userId);
        }

        void OnLBEActivation()
        {
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsColocatedDevice = true;

            DeviceDescriptionRequestDto deviceDescription = new DeviceDescriptionRequestDto()
            {
                macAddress = GetMacAddress(),
                deviceModel = SystemInfo.deviceModel,
                batteryLevel = SystemInfo.batteryLevel * 100
            };

            StartCalibrationScene();
            GetARPlanes();
            GetGuardianArea();
            AddAnchorGuardian();

            UserGuardianRequestDto guardianDto = CreateGuardianDto();

            UMI3DClientServer.SendRequest(deviceDescription, true);
            UMI3DClientServer.SendRequest(guardianDto, reliable: true);
        }

        void OnLBESetGroupReception(LBESetUserGroupDto dto)
        {
            if ((UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsColocatedDevice)
            {
                (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).LBEGroupId = dto.groupId;

                UMI3DClientServer.SendRequest(new LBEUserRegisterRequestDto() { groupId = dto.groupId }, true);

                OcclusionForColocatedUsers(dto.colocatedUserIds);
            }
        }

        void OnLBEGuardianReception(List<ARAnchorDto> anchors)
        {
            CreateGuardianServer(anchors);
        }

        //void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        //{
        //    StartCoroutine(GetARPlanes());
        //}

        private void OcclusionForColocatedUsers(List<ulong> newColocatedUsers)
        {
            if ((UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).HasImmersiveDevice || !(UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsColocatedDevice)
                return;

            foreach (ulong userId in newColocatedUsers)
            {
                AbstractSkeleton skeleton = CollaborationSkeletonsManager.Instance.GetCollaborativeSkeleton((UMI3DGlobalID.EnvironmentId, userId)) as AbstractSkeleton;

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
            if (skeleton.Bones.TryGetValue(boneType, out UnityTransformation boneTransform))
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

                capsule.transform.SetParent(skeleton.HipsAnchor);
                capsule.transform.localPosition = new Vector3(boneTransform.Position.x, 0f, boneTransform.Position.z);
                capsule.transform.localPosition = new Vector3(0f, 0f, 0f);
                capsule.transform.localRotation = boneTransform.Rotation;
                capsule.transform.localScale = new Vector3(capsule.transform.localScale.x / 1.25f, capsule.transform.localScale.y, capsule.transform.localScale.z / 1.25f); // Ajustez si nécessaire

                Renderer capsuleRenderer = capsule.GetComponent<Renderer>();

                if (capsuleRenderer != null && OcclusionMaterial != null)
                {
                    capsuleRenderer.material = OcclusionMaterial;
                }
            }
            else
                Debug.LogWarning("REMY -> Bone not found.");
        }

        private void ARPlanesActivation(bool activation)
        {
            for (int i = 0; i < planesToCalibrate.Count; i++)
            {
                planesToCalibrate[i].gameObject.SetActive(activation);
            }
        }

        public void GetARPlanes()
        {

            if (arPlaneManager == null)
            {
                Debug.LogError("REMY : ARPlaneManager est NULL !");
                return;
                
            }

            if (arPlaneManager.trackables.count == 0)
            {
                Debug.LogWarning("REMY : Aucun plan AR détecté après l'attente.");
                return;
            }

            ARPlaneFrom = FindARPlaneByType(ARPlaneFromType);
            ARPlaneTo = FindARPlaneByType(ARPlaneToType);

            /*if (automaticCalibration)
            {*/
                Debug.Log("REMY : plane in trackables count next -> " + planesToCalibrate.Count);

                if (ARPlaneFrom != null && ARPlaneTo != null) // On veut au moins 2 plans
                {

                    // Calcul de la direction entre la table et la fenêtre
                    Vector3 direction = CalculateDirection(ARPlaneFrom, ARPlaneTo);

                    // Création du calibrateur avec la position de la table et l'orientation vers la fenêtre
                    GameObject calibratorARPlane = new GameObject("Calibreur Plane");
                    calibratorARPlane.transform.position = ARPlaneFrom.transform.position; // Garder la position de la table
                    calibratorARPlane.transform.rotation = Quaternion.LookRotation(direction, Vector3.up); // Orienté vers la fenêtre

                    // Affecter le calibrateur au joueur
                    calibrator = calibratorARPlane.transform;
                    calibrator.transform.parent = Player.transform;

                    // Instancier des repères pour chaque plan détecté
                    for (int i = 0; i < planesToCalibrate.Count; i++)
                    {
                        Instantiate(Repere, planesToCalibrate[i].transform.position, calibratorARPlane.transform.rotation);

                        // Ajouter des repères aux sommets de chaque ARPlane
                        Mesh mesh = planesToCalibrate[i].GetComponent<MeshFilter>().mesh;
                        Vector3[] vertices = mesh.vertices;

                        foreach (var vertex in vertices)
                        {
                            Vector3 worldPosition = planesToCalibrate[i].transform.TransformPoint(vertex);
                            Instantiate(Repere, worldPosition, calibratorARPlane.transform.rotation);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Multiple ARPlane detected. Only Two ARPlanes should be selected to serve as a calibrator. Change your environment configuration");
                    //automaticCalibration = false;
                    //SetManualCalibrator();
                }
            /*}
            else
                SetManualCalibrator();*/

            //Envoi de la position du calibrator qui correspond à la position du spawner pour le recalculé dans la TPGroupé 
            OnPositionCalibratorStart?.Invoke(calibrator.transform.position);
        }

        private ARPlane FindARPlaneByType(ARPlaneType type)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                if (IsMatchingPlaneType(plane, type))
                {
                    planesToCalibrate.Add(plane);
                    return plane;
                }
            }
            return null; 
        }

        private bool IsMatchingPlaneType(ARPlane plane, ARPlaneType type)
        {
            switch (type)
            {
                case ARPlaneType.Table:
                    return plane.classification == PlaneClassification.Table;
                case ARPlaneType.Window:
                    return plane.classification == PlaneClassification.Window;
                case ARPlaneType.Floor:
                    return plane.classification == PlaneClassification.Floor;
                case ARPlaneType.Ceiling:
                    return plane.classification == PlaneClassification.Ceiling;
                case ARPlaneType.Wall:
                    return plane.classification == PlaneClassification.Wall;
                case ARPlaneType.Other:
                    return plane.classification == PlaneClassification.None;
                default:
                    return false;
            }
        }


        Vector3 CalculateDirection(ARPlane from, ARPlane to)
        {
            Vector3 direction = to.transform.position - from.transform.position;
            direction.y = 0; // On annule la hauteur pour ne garder que la direction horizontale
            return direction.normalized;
        }

        //public void ProcessIDSubmission(string id)
        //{
        //    uint parsedID;
        //    if (userGuardianDto != null)
        //    {
        //        if (uint.TryParse(id, out parsedID))
        //        {
        //            userGuardianDto.IDLbeGroup = parsedID;
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("UserguardianDto empty");
        //    }
        //}

        //[Obsolete]
        //public void ToggleCalibrationScene(bool value)
        //{
        //    automaticCalibration = value;
        //    if (automaticCalibration)
        //        SetARPlaneCalibrator();
        //    else
        //        SetManualCalibrator();
        //

        void OnLBELeaderReception(bool value)
        {
            isLeader = value;
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsLBEGroupLeader = value;

            UMI3DClientServer.SendRequest(new LBELeaderRegisterRequestDto() { groupId = (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).LBEGroupId }, true);

            if (value)
            {
                info[LocomotionNotificationKeys.Info.Controller] = Controller.RightHand;
                info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
                info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Enable;
                NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);

                info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftHand;
                info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
                info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Enable;
                NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);
            }
            else
            {
                info[LocomotionNotificationKeys.Info.Controller] = Controller.RightHand;
                info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
                info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Disable;
                NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);


                info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftHand;
                info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
                info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Disable;
                NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);
            }
        }

        //[Obsolete]
        //public void ToggleUserAdmin(bool value)
        //{
        //    isLeader = value;

        //    if(value)
        //    {
        //        info[LocomotionNotificationKeys.Info.Controller] = Controller.RightHand;
        //        info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
        //        info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Enable;
        //        NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);

        //        info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftHand;
        //        info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
        //        info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Enable;
        //        NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);
        //    }
        //    else
        //    {
        //        info[LocomotionNotificationKeys.Info.Controller] = Controller.RightHand;
        //        info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
        //        info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Disable;
        //        NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);


        //        info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftHand;
        //        info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
        //        info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Disable;
        //        NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);
        //    }
        //}

        //[Obsolete]
        //private void SetARPlaneCalibrator()
        //{
        //    ManualCalibrator.gameObject.SetActive(false);

        //    if (planesToCalibrate.Count > 0)
        //    {
        //        ARPlanesActivation(true);
                //calibrator = planesToCalibrate[0].transform;
        //        calibrator = ARPlaneFrom.transform;

        //    }

        //    else
        //    {
        //        automaticCalibration = false;
        //        SetManualCalibrator();
        //    }
        //}

        //[Obsolete]
        //private void SetManualCalibrator()
        //{
        //    ARPlanesActivation(false);

        //    ManualCalibrator.gameObject.SetActive(true);
        //    calibrator = ManualCalibrator.transform;

        //    if (OrientationScenePanel.gameObject.activeSelf == true && OrientationScenePanel.alpha == 1)
        //    {
        //        OrientationScenePanel.GetComponent<SetPlayerOrientationPanel>().ClosePanel();
        //        ButtonOrientationScene.onOffOrientationPanel = false;
        //    }
        //}

        public void StartCalibrationScene()
        {
            if ((UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DCollabLoadingParameters).IsColocatedDevice)
                //StartCoroutine(CalibrationScene());
                CalibrationScene();

        }

        string GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return nic.GetPhysicalAddress().ToString();
            }
            return "";
        }

        //[Obsolete]
        //public void OrientationChoice(float orientation)
        //{
        //    orientationOffset = orientation;
        //}

        //[Obsolete]
        //public void CloseOrientationChoice()
        //{
        //    calibrator.transform.Rotate(calibrator.transform.rotation.x, orientationOffset, calibrator.transform.rotation.z, Space.World);
        //    OrientationScenePanel.gameObject.SetActive(false);
        //}

        public void CalibrationScene()
        {
           /* yield return null;
            yield return null;*/

            // TODO check the reason we have to wait

            //if (automaticCalibration)
            //{
            //    CloseOrientationChoice();
            //}

            if (Player != null)
            {
                Transform scene = Player.transform.parent;
                Instantiate(Repere, scene.position, scene.rotation);

                if (scene != null)
                {

                    calibrator.transform.rotation = Quaternion.Euler(0.0f, calibrator.transform.rotation.eulerAngles.y, 0.0f);

                    calibrator.transform.SetParent(null, true);

                    Player.transform.SetParent(calibrator.transform, true);

                    Vector3 Offset = Vector3.ProjectOnPlane(CameraPlayer.transform.position - calibrator.transform.position, Vector3.up);
                    calibrator.transform.Translate(Offset, Space.World);

                    float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(calibrator.transform.forward, Vector3.up), Vector3.ProjectOnPlane(CameraPlayer.transform.forward, Vector3.up), Vector3.up);
                    calibrator.transform.Rotate(0f, angle, 0f);

                    Player.transform.SetParent(scene, true);
                    calibrator.transform.SetParent(Player.transform, true);

                    //Création du Parent des ancres
                    guardianParent = new GameObject("Guardian");
                    guardianParent.transform.position = new Vector3(calibrator.transform.position.x, 0.0f, calibrator.transform.position.z);

                    //GetGuardianArea();
                }
                else
                    Debug.LogError(Player.name + " has no parents.");
            }
            else
            {
                Debug.LogWarning("No GameObject to check is assigned!");
            }

            arPlaneManager.enabled = false;

            ARPlanesActivation(false);

            //AddAnchorGuardian();

            //SendGuardianInServer();

            //if (userGuardianDto != null)
            //{
            //    StartCoroutine(WaitSendGuardian());
            //}
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


                //Récupération des 4 points de la play area fournis par le guardian du casque
                if (inputSubsystem.TryGetBoundaryPoints(boundaryPoints))
                {
                    List<Vector3> MeshAnchor = new List<Vector3>();

                    List<GameObject> Reperes = new List<GameObject>();

                    if (AnchorManager != null)
                    {
                        foreach (Vector3 point in boundaryPoints)
                        {
                            Reperes.Add(Instantiate(Repere, point, Quaternion.identity));
                            Reperes.Add(Instantiate(Repere, point + new Vector3(0f, 2f, 0f), Quaternion.identity));

                            guardianAnchors.Add(point);
                            guardianAnchors.Add(point + new Vector3(0f, 2f, 0f));
                        }

                        //Déplacé les repères en xrorigin pour les tests
                        for (int i = 0; i < Reperes.Count; i++)
                        {
                            Reperes[i].transform.SetParent(XROrigin.transform, true);
                        }
                    }
                    else
                    {
                        Debug.LogError("AnchorManager not referenced !");
                    }

                    guardianMesh = new GameObject("GuardianMesh");

                    for (int i = 0; i < guardianAnchors.Count; i++)
                    {
                        Reperes[i].transform.SetParent(guardianMesh.transform, true);
                    }

                    guardianMesh.transform.position = Player.transform.position;
                    guardianMesh.transform.SetParent(Player.transform, true);

                    /* guardianMesh.transform.position = Vector3.zero;*/

                    CreateGuardianMesh(guardianAnchors);
                }
            }
            else
            {
                Debug.LogError("No XR input subsystem available.");
            }
        }

        // Envoyer les data de chaque ancres au serveur
        public UserGuardianRequestDto CreateGuardianDto()
        {
            if (guardianAnchors.Count > 0)
            {
                List<ARAnchorDto> anchors = new List<ARAnchorDto>();

                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    ARAnchorDto newAnchor = new ARAnchorDto();

                    newAnchor.position = new Vector3Dto { X = localVertexPositions[i].x, Y = localVertexPositions[i].y, Z = localVertexPositions[i].z };
                    newAnchor.rotation = new Vector4Dto { X = localVertexRotations[i].x, Y = localVertexRotations[i].y, Z = localVertexRotations[i].z, W = localVertexRotations[i].w };

                    anchors.Add(newAnchor);
                }
                //UMI3DLoadingParameters loadingParameters = UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters;
                //userGuardianDto.isImmersive = loadingParameters.HasImmersiveDevice;

                return new UserGuardianRequestDto() { aRAnchors = anchors };
            }

            return new UserGuardianRequestDto() { aRAnchors = new List<ARAnchorDto>() };
        }

        //IEnumerator WaitSendGuardian()
        //{
        //    yield return new WaitForSeconds(2f);
        //    UMI3DClientServer.SendRequest(userGuardianDto, reliable: true);
        //}

        public void CreateGuardianServer(List<ARAnchorDto> GuardianDto)
        {
            // Clear the client's first connection data
            if (guardianMesh != null)
            {
                Destroy(guardianMesh);
                guardianMesh = null;
            }

            guardianAnchors.Clear();
            localVertexPositions.Clear();
            localVertexRotations.Clear();
            tempVerticesTransform.Clear();

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
            try
            {
                //Define instantiated objects representing the guardian as ARAnchors
                for (int i = 0; i < guardianAnchors.Count; i++)
                {
                    Vector3 basePointPosition = guardianAnchors[i];
                    Quaternion basePointRotation = new Quaternion(0f, 0f, 0f, 0f);
                    Pose basePointPose = new Pose(basePointPosition, basePointRotation);
                }
                guardianMesh.AddComponent<ARAnchor>();
            }
            catch 
            {
                Debug.LogError("AddAnchorGuardian failed");
            }
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
                AnchorGuardianTemp.transform.parent = guardianMesh.transform;
                AnchorGuardianTemp.transform.localPosition = points[i];

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

            //guardianMesh.transform.position = Quaternion.Inverse(PersonnalSkeletonContainer.transform.rotation) * (guardianMesh.transform.position) + PersonnalSkeletonContainer.transform.position;
            guardianMesh.transform.rotation = PersonnalSkeletonContainer.transform.rotation;

            guardianMesh.AddComponent<HoverGuardian>().targetMaterial = GuardianMaterial;

            calibrator.transform.position = new Vector3(calibrator.transform.position.x, 0.0f, calibrator.transform.position.z);

            for (int i = 0; i < tempVerticesTransform.Count; i++)
            {
                tempVerticesTransform[i].transform.parent = calibrator.transform;

                localVertexPositions.Add(new Vector3(tempVerticesTransform[i].transform.localPosition.x, tempVerticesTransform[i].transform.localPosition.y, tempVerticesTransform[i].transform.localPosition.z));
                localVertexRotations.Add(tempVerticesTransform[i].transform.localRotation);
            }
            guardianMesh.transform.SetParent(XROrigin.transform, true);

        }
    }
}
#endregion