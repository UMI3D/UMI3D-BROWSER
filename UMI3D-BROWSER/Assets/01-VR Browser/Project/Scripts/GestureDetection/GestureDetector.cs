
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
}

[System.Serializable]
public class Hand
{
    [SerializeField] private float _threshold = 0.05f;
    [SerializeField] private GestureSet _gestureSet;

    private Transform _skeletonTransfom;
    private List<Transform> _jointTransform;

    private Gesture _currentGesture;
    private Gesture _previousGesture;

    public Gesture CurrentGesture => _currentGesture;
    public event Action<Gesture> NewActionDetected;

    public void Setup(GameObject handRoot, XRHandSkeletonDriver handSkeleton)
    {
        // Get Base Transform
        for (var childIndex = 0; childIndex < handRoot.transform.childCount; ++childIndex)
        {
            var child = handRoot.transform.GetChild(childIndex);
            if (child.gameObject.name.EndsWith(XRHandJointID.Wrist.ToString()))
            {
                _skeletonTransfom = child;
            }
        }

        // Get Transform of each bones
        _jointTransform = new();
        foreach (var joint in handSkeleton.jointTransformReferences)
        {
            _jointTransform.Add(joint.jointTransform);
        }
    }

    public void Update()
    {
        if (_skeletonTransfom == null) return;

        _currentGesture = Recognize();

        if (!_currentGesture.Equals(_previousGesture))
            NewActionDetected?.Invoke(_currentGesture);

        _previousGesture = _currentGesture;
    }

    private Gesture Recognize()
    {
        var currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;
        Debug.Log(Vector3.Angle(Vector3.up, _skeletonTransfom.rotation.eulerAngles));

        foreach (var gesture in _gestureSet.Gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < _jointTransform.Count; i++)
            {
                Vector3 currentData = _skeletonTransfom.transform.InverseTransformPoint(_jointTransform[i].transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > _threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                sumDistance = currentMin;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }

    #region Save
    public void Save()
    {
        if (_gestureSet == null)
        {
            Debug.LogError("Gesture a saved in a gesture set, so it must not be null!");
            return;
        }
        if (_skeletonTransfom == null)
        {
            Debug.LogError("Must start the scene to create a new gesture!");
            return;
        }

        var data = new List<Vector3>();
        foreach (var joint in _jointTransform)
        {
            data.Add(_skeletonTransfom.transform.InverseTransformPoint(joint.transform.position));
        }

        var gesture = new Gesture();
        gesture.name = "New Gesture";
        gesture.fingerDatas = data;
        _gestureSet.Gestures.Add(gesture);
    }
    #endregion
}

public class GestureDetector : MonoBehaviour
{
    [SerializeField] private HandVisualizer _handVisualizer;
    [Header("Hands")]
    [SerializeField] private Hand _rightHand;
    [SerializeField] private Hand _leftHand;

    private bool _isInitialized = false;

    public Hand RightHand => _rightHand;
    public Hand LeftHand => _leftHand;
    public bool IsInitialized => _isInitialized;

    private void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        // todo : SETUP AGAIN
        //yield return new WaitUntil(() => _handVisualizer.RightHandGameObjects != null);
        //yield return new WaitUntil(() => _handVisualizer.RightHandGameObjects.HandSkeleton != null);

        //_rightHand.Setup(_handVisualizer.RightHandGameObjects.HandRoot, _handVisualizer.RightHandGameObjects.HandSkeleton);
        //_leftHand.Setup(_handVisualizer.LeftHandGameObjects.HandRoot, _handVisualizer.LeftHandGameObjects.HandSkeleton);
        yield return null;
        _isInitialized = true;
    }

    private void Update()
    {
        _rightHand.Update();
        _leftHand.Update();
    }
}
